﻿//#define SAFE_MODE

//#define DEBUG_COMPARE_STEPS
//#define DEBUG_RESULT_DETAILED
//#define DEBUG_RESULT
//#define DEBUG_SORT

using System;
using UnityEngine;

#if DEV_MODE && (DEBUG_RESULT_DETAILED || DEBUG_RESULT)
using System.Collections.Generic;
#endif

namespace Sisus
{
	[Serializable]
	public class FuzzyComparable : IComparable, IComparable<FuzzyComparable>
	{
		private const int Lower = 0;
		private const int Upper = 1;
		private const int NonLetter = 2;

		private const char Space = ' ';
		private const char Underscore = '_';

		public static readonly FuzzyComparable Empty = new FuzzyComparable("");

		#if DEV_MODE && (DEBUG_RESULT_DETAILED || DEBUG_RESULT)
		private static readonly List<string> debugWords = new List<string>();
		private static readonly List<string> debugResults = new List<string>();
		#endif

		public readonly string text;

		/// <summary>
		/// Used by SortBySearchStringMatchness, indicates how good a match
		/// this was when compared against the search string FuzzyComparable.
		/// The searchMatch values of multiple FuzzyComparables are then
		/// used in sorting them.
		/// </summary>
		public int searchMatch;

		/// <summary>
		/// Text converted into numeric form optimized for fast fuzzy comparison with other FuzzyComparables. </summary>
		[SerializeField]
		private readonly int[] compare;

		/// <summary>
		/// indexes in compare that contain spaces, slasher or other characters that indicate
		/// that one part of the filter ends and another begins changes and another begins.
		/// </summary>
		[SerializeField]
		private readonly int[] splitPoints;

		private readonly int mainPartIndex;

		private int Length
		{
			get
			{
				return compare.Length;
			}
		}
		
		public FuzzyComparable(string textInput)
		{
			text = textInput;
			compare = FuzzyComparableBuilder.GenerateFuzzyComparableData(textInput, out splitPoints, out mainPartIndex);
		}
		
		/// <summary> Gets sub-section of compare array from "from" index to "to" index and outputs it to the result int array. </summary>
		/// <param name="from"> Source index. </param>
		/// <param name="to"> to index. </param>
		/// <param name="result"> [in,out] The result. </param>
		private void GetCompareSubsection(int from, int to, ref int[] result)
		{
			int size = to - from;
			ArrayPool<int>.Resize(ref result, size);
			for(int n = 0; n < size; n++)
			{
				result[n] = compare[from + n];
			}
		}

		public void GetCompareBodyWithoutSplitPoints(ref int[] result)
		{
			int splitCount = splitPoints.Length;
			int compareSize = compare.Length;

			if(splitCount == 0)
			{
				ArrayPool<int>.Resize(ref result, compareSize);
				for(int n = compareSize - 1; n >= 0; n--)
				{
					result[n] = compare[n];
				}
				return;
			}

			int resultSize = compareSize - splitCount;
			ArrayPool<int>.Resize(ref result, resultSize);
			int index = 0;
			int splitPointIndex = 0;
			int splitPoint = splitPoints[0];
			for(int n = 0; n < compareSize; n++)
			{
				if(n == splitPoint)
				{
					splitPointIndex++;
					if(splitPointIndex < splitCount)
					{
						splitPoint = splitPoints[splitPointIndex];
					}
					else
					{
						splitPointIndex = -1;
					}
					continue;
				}

				result[index] = compare[n];
				index++;
			}
		}

		public static string CharInArrayToString(int[] ints)
		{
			int count = ints.Length;
			var chars  = ArrayPool<char>.Create(count);
			for(int n = 0; n < count; n++)
			{
				char c = (char)ints[n];
				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(c != '\0', "ints["+n+"] / "+n+" was null!");
				#endif
				chars[n] = c == '\0' ? '~' : c;
			}
			var result = new string(chars);
			ArrayPool<char>.Dispose(ref chars);
			return result;
		}
		
		public int CompareTo(object obj)
		{
			var other = (FuzzyComparable)obj;
			return searchMatch.CompareTo(other.searchMatch);
		}
			
		public int CompareTo(FuzzyComparable obj)
		{
			return searchMatch.CompareTo(obj.searchMatch);
		}

		/// <summary>
		/// The smaller the result, the closer the match
		/// </summary>
		public static int FuzzyCompare(FuzzyComparable searchString, FuzzyComparable testAgainst)
		{
			int sCount = searchString.splitPoints.Length;

			int finalResult = 0;
			int[] sCompare = ArrayPool<int>.Create(0);
			int sFrom = 0;
			int sTo;
			int wordResult;
			for(int s = 0; s < sCount; s++)
			{
				sTo = searchString.splitPoints[s];
				searchString.GetCompareSubsection(sFrom, sTo, ref sCompare);

				#if DEV_MODE
				//I think this can happen when a string starts with a space, ends with a space, or has multiple spaces in a row
				Debug.Assert(sCompare.Length > 0, "sCompare was empty with sFrom=" + sFrom + ", sTo=" + sTo + ", searchString=\"" + searchString.text+"\"");
				#endif

				wordResult = FuzzyCompareWord(sCompare, testAgainst);
				finalResult += wordResult;

				
				sFrom = sTo + 1;
			}
			sTo = searchString.Length;
			searchString.GetCompareSubsection(sFrom, sTo, ref sCompare);
			wordResult = FuzzyCompareWord(sCompare, testAgainst);

			finalResult += wordResult;
			finalResult /= sCount + 1;

			#if DEV_MODE && (DEBUG_RESULT || DEBUG_RESULT_DETAILED)
			debugWords.Sort();
			debugResults.Add("----------FINAL RESULT: "+finalResult.ToString("D4")+" \""+testAgainst.text+"\".FuzzyCompare(\""+searchString.text+"\")----------\n" + string.Join("\n", debugWords.ToArray()));
			debugWords.Clear();
			#endif

			ArrayPool<int>.Dispose(ref sCompare);

			return finalResult;
		}

		/// <summary>
		/// The smaller the result, the closer the match
		/// </summary>
		public static int FuzzyCompareWord(int[] sCompare, FuzzyComparable testAgainst)
		{
			#if DEV_MODE && DEBUG_RESULT_DETAILED
			var debugSteps = "";
			#endif

			int splitPointCount = testAgainst.splitPoints.Length;
			int stepResult;
			int[] tCompare = ArrayPool<int>.Create(0);
			int tFrom = 0;
			int tTo;
			int wordResult = int.MaxValue;
			for(int s = 0; s < splitPointCount; s++)
			{
				tTo = testAgainst.splitPoints[s];

				if(tTo > tFrom)
				{
					testAgainst.GetCompareSubsection(tFrom, tTo, ref tCompare);

					#if DEV_MODE && PI_ASSERTATIONS
					//I think this can happen when a string starts with a space, ends with a space, or has multiple spaces in a row
					if(tCompare.Length == 0) { Debug.LogError("tCompare was empty with tFrom="+tFrom+", tTo="+tTo+ ", testAgainst=\"" + testAgainst.text + "\" (WORD NOW: " + wordResult + ")"); }
					if(Array.IndexOf(tCompare, ' ') != -1) { Debug.LogError(CharInArrayToString(tCompare)); }
					#endif

					if(tCompare.Length > 0)
					{
						stepResult = FuzzyCompare(sCompare, tCompare, s == testAgainst.mainPartIndex);
						if(stepResult < wordResult)
						{
							wordResult = stepResult;
						}
					}
					#if DEV_MODE && DEBUG_RESULT_DETAILED
					else
					{
						stepResult = int.MaxValue;
					}
					debugSteps += "\n\""+testAgainst.text+"\".GetCompareSubsection(\""+CharInArrayToString(sCompare)+"\") vs \""+CharInArrayToString(tCompare)+"\" STEP: "+stepResult + " (WORD NOW: "+wordResult+")";
					#endif
				}

				tFrom = tTo + 1;
			}

			tTo = testAgainst.Length;
			testAgainst.GetCompareSubsection(tFrom, tTo, ref tCompare);
			if(tCompare.Length > 0)
			{
				stepResult = FuzzyCompare(sCompare, tCompare, testAgainst.mainPartIndex == splitPointCount);

				// Slightly favor the main part when matching.
				if(testAgainst.mainPartIndex == splitPointCount)
				{
					stepResult -= 1;
				}

				if(stepResult < wordResult)
				{
					wordResult = stepResult;
				}
			}
			#if DEV_MODE && DEBUG_RESULT_DETAILED
			else
			{
				stepResult = int.MaxValue;
			}
			debugSteps += "\n\""+testAgainst.text+"\".GetCompareSubsection(\""+CharInArrayToString(sCompare)+"\") vs \""+CharInArrayToString(tCompare)+"\" STEP: "+stepResult + " (WORD NOW: "+wordResult+")";
			#endif

			// UPDATE: Also test against entire compare body while ignoring any split points.
			// This way e.g. camerahandler will match "Camera Handler".
			if(testAgainst.splitPoints.Length > 0)
			{
				testAgainst.GetCompareBodyWithoutSplitPoints(ref tCompare);
				if(tCompare.Length > 0)
				{
					stepResult = FuzzyCompare(sCompare, tCompare, testAgainst.mainPartIndex == 0);
					if(stepResult < wordResult)
					{
						wordResult = stepResult;
					}
				}
				#if DEV_MODE && DEBUG_RESULT_DETAILED
				else
				{
					stepResult = int.MaxValue;
				}
				debugSteps += "\n\""+testAgainst.text+"\".GetCompareSubsection(\""+CharInArrayToString(sCompare)+"\") vs \""+CharInArrayToString(tCompare)+"\" STEP: "+stepResult + " (WORD NOW: "+wordResult+")";
				#endif
			}

			#if DEV_MODE && DEBUG_RESULT_DETAILED
			debugSteps += "\n\""+testAgainst.text+"\".GetCompareSubsection(\""+CharInArrayToString(sCompare)+"\") vs \""+CharInArrayToString(tCompare)+"\" STEP: "+stepResult + " (WORD NOW: "+wordResult+")";
			debugWords.Add("\""+testAgainst.text+"\" WORD RESULT: "+wordResult + " for word "+CharInArrayToString(sCompare) + debugSteps);
			#endif

			ArrayPool<int>.Dispose(ref tCompare);

			return wordResult;
		}
		
		public static int FuzzyCompare(int[] searchString, int[] testAgainst, bool isMainPart)
		{
			int searchCount = searchString.Length;
			int testCount = testAgainst.Length;
			if(searchCount == 0)
			{
				#if DEV_MODE && DEBUG_COMPARE_STEPS
				Debug.Log("searchCount == 0 => \""+CharInArrayToString(testAgainst)+"\" vs \""+CharInArrayToString(searchString)+"\" result = "+testCount);
				#endif

				return testCount;
			}
			if(testCount == 0)
			{
				#if DEV_MODE && DEBUG_COMPARE_STEPS
				Debug.Log("testCount == 0 => \""+CharInArrayToString(testAgainst)+"\" vs \""+CharInArrayToString(searchString)+"\" result = "+searchCount);
				#endif

				return searchCount;
			}

			int initialLettersMatch = 0;
			int partBonus = isMainPart ? -500 : 0;

			//if search string matches exactly the beginning portion of the target string, adjust result by - 1000
			//to make sure it'll be one of the top results
			if(searchCount <= testCount)
			{
				for(int n = 0; n < searchCount && searchString[n].Equals(testAgainst[n]); n++)
				{
					initialLettersMatch++;
				}

				if(initialLettersMatch == searchCount)
				{
					#if DEV_MODE && DEBUG_COMPARE_STEPS
					Debug.Log("initialLettersMatch == searchCount => \""+CharInArrayToString(testAgainst)+"\" vs \""+CharInArrayToString(searchString)+"\" result = "+((testCount - searchCount) - 1000));
					#endif

					return (testCount - searchCount) - 1000 + partBonus;
				}

				#if DEV_MODE && DEBUG_COMPARE_STEPS
				Debug.Log("\""+CharInArrayToString(testAgainst)+"\" vs \""+CharInArrayToString(searchString)+"\" match: "+initialLettersMatch);
				#endif
				
				//if search string is a substring of the target string, adjust result by - 500
				//to make it rise higer in the top results
				int stop = testCount - searchCount;
				for(int testIndex = 0; testIndex <= stop; testIndex++)
				{
					int thisMatch = 0;
					for(int n = 0; n < searchCount && searchString[n].Equals(testAgainst[testIndex+n]); n++)
					{
						thisMatch++;
					}

					if(thisMatch > initialLettersMatch)
					{
						initialLettersMatch = thisMatch;
					}

					#if DEV_MODE && DEBUG_COMPARE_STEPS
					Debug.Log("\""+CharInArrayToString(testAgainst)+"\" vs \""+CharInArrayToString(searchString)+"\" substring \""+CharInArrayToString(testAgainst).Substring(testIndex, searchCount)+"\" match: "+thisMatch);
					#endif
				}

				if(initialLettersMatch == searchCount)
				{
					#if DEV_MODE && DEBUG_COMPARE_STEPS
					Debug.Log("substringLettersMatch == searchCount => \""+CharInArrayToString(testAgainst)+"\" vs \""+CharInArrayToString(searchString)+"\" result = "+((testCount - searchCount) - 500));
					#endif

					return (testCount - searchCount) - 500 + partBonus;
				}
			}
				
			// Rather than maintain an entire matrix (which would require O(xCount*yCount) space),
			// just store the current row and the next row, each of which has a length yCount+1,
			// so just O(yCount) space. Initialize the current row.
			int curRow = 0, nextRow = 1;
			int[][] rows = { new int[testCount + 1], new int[testCount + 1] };
			for(int j = 0; j <= testCount; ++j)
			{
				rows[curRow][j] = j;
			}

			// For each virtual row (since we only have physical storage for two)
			for(int i = 1; i <= searchCount; ++i)
			{
				// Fill in the values in the row
				rows[nextRow][0] = i;
				for(int j = 1; j <= testCount; ++j)
				{
					int dist1 = rows[curRow][j] + 1;
					int dist2 = rows[nextRow][j - 1] + 1;
					int dist3 = rows[curRow][j - 1] + (searchString[i - 1].Equals(testAgainst[j - 1]) ? 0 : 1);
					rows[nextRow][j] = Math.Min(dist1, Math.Min(dist2, dist3));
				}

				// Swap the current and next rows
				if(curRow == 0)
				{
					curRow = 1;
					nextRow = 0;
				}
				else
				{
					curRow = 0;
					nextRow = 1;
				}
			}

			int dist = rows[curRow][testCount];
			
			#if DEV_MODE && DEBUG_COMPARE_STEPS
			Debug.Log("default => \""+CharInArrayToString(testAgainst)+"\" vs \""+CharInArrayToString(searchString)+"\" result = "+(dist + dist - initialLettersMatch));
			#endif

			return dist + dist - initialLettersMatch + partBonus;
		}

		public static void SortBySearchStringMatchness(ref FuzzyComparable searchString, ref FuzzyComparable[] valuesToSort)
		{
			int count = valuesToSort.Length;
			for(int n = count - 1; n >= 0; n--)
			{
				var value = valuesToSort[n];
				int dist = FuzzyCompare(searchString, value);
				value.searchMatch = dist;
			}

			Array.Sort(valuesToSort);

			#if DEV_MODE && DEBUG_SORT
			Debug.Log("Sorted for \""+searchString.text+"\":\n"+StringUtils.ToString(valuesToSort, "\n"));
			#endif

			#if DEV_MODE && (DEBUG_RESULT_DETAILED || DEBUG_RESULT)
			debugResults.Sort();
			for(int d = 0; d < debugResults.Count; d++)
			{
				Debug.Log(debugResults[d]);
			}
			debugResults.Clear();
			#endif
		}

		public override string ToString()
		{
			return StringUtils.Concat(text, " (", searchMatch, ")");
		}
	}
}