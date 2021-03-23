#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using FluentAssertions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Utils.ProceduralAnimationLibrary.Cutscenes
{
    public static class CutsceneParser
    {
        public static ITweener Parse(string text)
        {
            VerbLoader.Initialize();
            var result = new SerialTweener();

            foreach (var line in text.Split('\n')
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Where(line => !line.StartsWith("#")))
            {
                var subjectMatch = Regex.Match(line, @"^\s*(?<Subject>\w+)");
                subjectMatch.Success.Should().BeTrue();
                var remainder = line.Substring(subjectMatch.Length).Trim();
                var subjectString = subjectMatch.Groups["Subject"].Value;

                GameObject? subject = null;
                VerbDetails? verbDetails = null;
                if (VerbLoader.Verbs.TryGetValue(subjectString, out var subjectlessVerb)
                    && subjectlessVerb.NoSubject)
                {
                    verbDetails = subjectlessVerb;
                }
                else
                {
                    var verbMatch = Regex.Match(remainder, @"(?<Verb>\w+)");
                    verbMatch.Success.Should().BeTrue();
                    subject = GameObject.Find(subjectString);
                    subject.Should().NotBeNull();
                    var verbString = verbMatch.Groups["Verb"].Value;
                    var verbFound = VerbLoader.Verbs.TryGetValue(verbString, out verbDetails);
                    verbFound.Should().BeTrue();
                    remainder = remainder.Substring(verbMatch.Length).Trim();
                }
                
                if (verbDetails.DirectObjectIsString)
                {
                    var temp = verbDetails.Action.Invoke(subject!, remainder,
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

                var temp2 = verbDetails.Action.Invoke(subject!, directObject, parameters);
                result.Append(temp2);
            }

            return result;
        }
    }
}