using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Assets.Utils;
using Assets.Utils.JBehavior;
using FluentAssertions;
using UnityEngine;

namespace Assets.Scripts.Cutscene
{
    public class VerbSetObject
    {
        public readonly Func<GameObject, object, Dictionary<string, string>, JBehaviorSet> Action;
        public readonly bool DirectObjectIsString;

        public VerbSetObject(Func<GameObject, object, Dictionary<string, string>, JBehaviorSet> action, bool directObjectIsString)
        {
            Action = action;
            DirectObjectIsString = directObjectIsString;
        }
    }

    public class CutsceneAnimator
    {
        private readonly TextAsset Asset;

        public Dictionary<string, VerbSetObject> VerbSet =
            new Dictionary<string, VerbSetObject>(
                new EZComparer<string>((s1, s2) => string.Compare(s1, s2, StringComparison.InvariantCultureIgnoreCase) == 0,
                    s => s.ToUpper().GetHashCode())
                );

        public CutsceneAnimator(TextAsset asset)
        {
            Asset = asset;

            var verbs = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Select(m => new
                {
                    Method = m,
                    Action = m.GetCustomAttribute<ActionAttribute>()
                })
                .Where(it => it.Action != null);

            foreach (var verb in verbs)
            {
                var expecteds = verb.Method.GetParameters();

                expecteds[0].Name.Should().Be("subject");
                expecteds[0].ParameterType.Should().Be(typeof(GameObject));
                expecteds[1].Name.Should().Be("directObject");
                var doIsString = false;
                if (expecteds[1].ParameterType == typeof(GameObject))
                {
                    // pass
                }
                else if (expecteds[1].ParameterType == typeof(string))
                {
                    doIsString = true;
                }
                else
                {
                    throw new NotImplementedException("DirectObject should be GameObject or String");
                }

                Func<GameObject, object, Dictionary<string, string>, JBehaviorSet> action = (subject, directObject, parameters) =>
                {
                    var actuals = new List<object> { subject };

                    if (doIsString)
                    {
                        directObject.Should().BeOfType<string>();
                        actuals.Add(directObject);
                    }
                    else
                    {
                        directObject.Should().BeOfType<GameObject>();
                        actuals.Add(directObject);
                    }

                    foreach (var expected in expecteds.Skip(2))
                    {
                        if (parameters.TryGetValue(expected.Name, out var actualString))
                        {
                            if (expected.ParameterType == typeof(float?))
                            {
                                if (float.TryParse(actualString, out var actualFloat))
                                {
                                    actuals.Add(actualFloat);
                                    continue;
                                }
                            }
                            throw new NotImplementedException();
                        }

                        actuals.Add(null);
                    }

                    return (JBehaviorSet)verb.Method.Invoke(this, actuals.ToArray());
                };

                var name = verb.Action.Name ?? verb.Method.Name;
                VerbSet[name] = new VerbSetObject(action, doIsString);
            }
        }

        public JBehaviorSet Create()
        {
            var result = new JBehaviorSet();

            foreach (var line in Asset.text.Split('\n')
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Where(line => !line.StartsWith("#")))
            {
                var subjectVerbMatch = Regex.Match(line, @"^\s*(?<Subject>\w+)\s+(?<Verb>\w+)");
                subjectVerbMatch.Success.Should().BeTrue("Unknown line!");
                var subjectString = subjectVerbMatch.Groups["Subject"].Value;
                var subject = GameObject.Find(subjectString);
                subject.Should().NotBeNull();
                var verbString = subjectVerbMatch.Groups["Verb"].Value;
                var verbFound = VerbSet.TryGetValue(verbString, out var verbAction);
                verbFound.Should().BeTrue();
                var remainder = line.Substring(subjectVerbMatch.Length).Trim();
                if (verbAction.DirectObjectIsString)
                {
                    var temp = verbAction.Action.Invoke(subject, remainder,
                        new Dictionary<string, string>());
                    result.Append(temp);
                    continue;
                }

                var remainderMatch = Regex.Match(remainder, @"(?<DirectObject>\w+)\s*(\swith\s+(?<Parameters>.*))?$");
                remainderMatch.Success.Should().BeTrue();

                var t2 = remainderMatch.Groups["DirectObject"].Value;
                var t3 = remainderMatch.Groups["Parameters"].Value ?? "";
                var directObject = GameObject.Find(t2);
                var parameters =
                    !string.IsNullOrWhiteSpace(t3)
                        ? t3.Split(',')
                            .Select(it => Regex.Split(it, @"\s+"))
                            .ToDictionary(it => it[0], it => it[1])
                        : new Dictionary<string, string>();

                var temp2 = verbAction.Action.Invoke(subject, directObject, parameters);
                result.Append(temp2);
            }

            return result;
        }

        [Action]
        private JBehaviorSet MoveTo(GameObject subject, GameObject directObject, float? speed)
        {
            var velocity = speed ?? 5f;

            return new JBehaviorSet().Computed(
                () => new
                {
                    position = subject.transform.position,
                    target = directObject.transform.position.ReplaceY(subject.transform.position.y)
                },
                initial => Vector3.Distance(initial.position, initial.target) / velocity,
                (initial, ratio) =>
                {
                    subject.transform.position = initial.position + ratio * (initial.target - initial.position);
                });
        }

        [Action]
        private JBehaviorSet Face(GameObject subject, GameObject directObject, float? degreesPerSecond)
        {
            var velocity = degreesPerSecond ?? 360f;

            return new JBehaviorSet().Computed(
                () =>
                {
                    // Rotate subject to face directObject
                    var vec = subject.transform.forward;
                    var dir = directObject.transform.position - subject.transform.position;
                    var cross = Vector3.Cross(vec, dir);
                    var angle = Vector3.Angle(vec, dir);

                    return new
                    {
                        facing = subject.transform.rotation,
                        angle,
                        sign = Mathf.Sign(cross.y)
                    };
                },
                initial => initial.angle / velocity,
                (initial, ratio) =>
                {
                    subject.transform.rotation = initial.facing *
                                                 Quaternion.AngleAxis(ratio * initial.angle * initial.sign, Vector3.up);
                });
        }

        [Action]
        private JBehaviorSet Say(GameObject subject, string directObject)
        {
            var unsay = Sayer.Say(subject, directObject);
            return new JBehaviorSet(); // .Wait(2.0f).Then(() => unsay());
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ActionAttribute : Attribute
    {
        public readonly string Name;

        public ActionAttribute(string verb = null)
        {
            Name = verb;
        }
    }
}