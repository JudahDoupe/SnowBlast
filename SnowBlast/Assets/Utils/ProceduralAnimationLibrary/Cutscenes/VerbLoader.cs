#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Utils.ProceduralAnimationLibrary.Tweens;
using FluentAssertions;
using UnityEngine;

namespace Assets.Utils.ProceduralAnimationLibrary.Cutscenes
{
    public static class VerbLoader
    {
        public static Dictionary<string, VerbDetails> Verbs =
            new Dictionary<string, VerbDetails>(
                new EZComparer<string>((s1, s2) => string.Compare(s1, s2, StringComparison.InvariantCultureIgnoreCase) == 0,
                    s => s.ToUpper().GetHashCode())
            );

        public static bool Initialized;

        public static void Initialize()
        {
            if (!Initialized)
            {
                Initialized = true;
                AddVerbsFromType(typeof(BuiltInVerbs));
            }
        }

        public static void AddVerbsFromType(Type type)
        {
            var verbs = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .Select(m => new
                {
                    Method = m,
                    Action = m.GetCustomAttribute<CutsceneVerbAttribute>()
                })
                .Where(it => it.Action != null);

            foreach (var verb in verbs)
            {
                var expectedParameters = new Queue<ParameterInfo>(verb.Method.GetParameters());

                if (!verb.Action.NoSubject)
                {
                    var subjectParam = expectedParameters.Dequeue();
                    subjectParam.Name.Should().Be("subject");
                    subjectParam.ParameterType.Should().Be(typeof(GameObject));
                }

                var doParam = expectedParameters.Dequeue();
                
                doParam.Name.Should().Be("directObject");
                var doIsString = false;
                if (doParam.ParameterType == typeof(GameObject))
                {
                    // pass
                }
                else if (doParam.ParameterType == typeof(string))
                {
                    doIsString = true;
                }
                else
                {
                    throw new NotImplementedException("DirectObject should be GameObject or String");
                }

                Func<GameObject, object, Dictionary<string, string>, ITween> action = (subject, directObject, parameters) =>
                {

                    var actuals = new List<object?>();

                    if (!verb.Action.NoSubject)
                    {
                        actuals.Add(subject);
                    }

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

                    foreach (var expected in expectedParameters)
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

                    return (ITween)verb.Method.Invoke(null, actuals.ToArray());
                };

                var name = verb.Action.Name ?? verb.Method.Name;
                Verbs[name] = new VerbDetails(action, doIsString, verb.Action.NoSubject);
            }
        }
    }
}