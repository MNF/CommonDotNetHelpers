 #region  Namespace Imports
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualBasic.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Diagnostics;
    #endregion  //Namespace Imports

        //See also DataHelper.DateEndOfDay etc
        /// <summary> 
        /// TODO: add more functionality from http://stackoverflow.com/questions/4781611/how-to-know-if-a-datetime-is-between-a-daterange-in-c-sharp
        /// </summary>
        public struct DateTimeRange
        {
            public DateTime DateTimeFrom;
            public DateTime DateTimeTo;
	
            /// <summary>
            /// Pass From DateTime values unchanged
            /// </summary>
            /// <param name="dtFrom"></param>
            /// <param name="dtTo"></param>
            public DateTimeRange(DateTime dtFrom, DateTime dtTo)
            {
                DateTimeFrom = dtFrom;// DateType.FromObject(dtFrom);
                DateTimeTo = dtTo;// DateType.FromObject(dtTo);
            }
            public DateTimeRange(DateTime dateFrom, DateTime dateTo, DateTime TimeFrom, DateTime TimeTo)
            {
                DateTimeFrom = DateTimeHelper.DateAndTime(dateFrom, DateTimeHelper.TimeToDateTime(TimeFrom));
                DateTimeTo = DateTimeHelper.DateAndTime(dateTo, DateTimeHelper.TimeToDateTime(TimeTo));
            }
            public DateTimeRange(DateTime dateFrom, DateTime dateTo, TimeSpan TimeFrom, TimeSpan TimeTo)
            {
                DateTimeFrom = DateTimeHelper.DateAndTime(dateFrom, DateTimeHelper.TimeToDateTime(TimeFrom));
                DateTimeTo = DateTimeHelper.DateAndTime(dateTo, DateTimeHelper.TimeToDateTime(TimeTo));
            }
            /// <summary>
            /// SetDateRange assumes that parameteres are date only and sets timePart of From to 0 and To to 23.59
            /// </summary>
            /// <param name="DateFrom"></param>
            /// <param name="DateTo"></param>
            public void SetDateRange(object DateFrom, object DateTo)
            {
                DateTimeFrom = DateTimeHelper.DateStartOfDay(DateType.FromObject(DateFrom));
                DateTimeTo = DateTimeHelper.DateEndOfDay(DateType.FromObject(DateTo));
            }
            public string Validate()
            {
                if (DataHelper.IsDBNullOrEmpty(DateTimeFrom) == true)
                    return "FROM Date/Time is not specified";
                if (DataHelper.IsDBNullOrEmpty(DateTimeFrom) == true)
                    return "TO Date/Time is not specified";
                if (DateTime.Compare(DateTimeFrom, DateTimeTo) > 0)
                {
                    return "From Date/Time greater than To Date/Time";
                }
                return "";
            }
            public bool GetSQLStrings(ref string sdtFrom, ref string sdtTo)
            {
                sdtFrom = DateTimeHelper.FormatDateTimeSQL(DateTimeFrom);
                sdtTo = DateTimeHelper.FormatDateTimeSQL(DateTimeTo);
                return true;
            }
            public override string ToString()
            {
                string sRet = String.Format("From {0} to {1}", DateTimeFrom.ToString(), DateTimeTo.ToString());
                return sRet;
            }
#region  Static Public methods
            /// <summary>
            /// 
            /// </summary>
            /// <param name="listOfDates">Expect ascending sequence of dates</param>
            /// <returns>List of ranges for contiguous sequense</returns>
            /// <remarks>E.g for sequence 1,2,3, 6,7,8,9, 11,12, 14, 18 it should return ranges
            /// (1,3) ,(6,9), (11,12), (14,14), (18,18)
            /// </remarks>
            public static List<DateTimeRange> BuildContiguousGroupsList(List<DateTime> listOfDates)
            {
                Debug.Assert(!CollectionsHelper.IsNullOrEmpty(listOfDates), "Investigate why?.  .");
                DateTime startOfRange = DateTime.MinValue;
                DateTime prevItem = DateTime.MinValue;
                List<DateTimeRange> listOfRanges = new List<DateTimeRange>();
                for(int i=0;i< listOfDates.Count;i++)
                {
                    DateTime item = listOfDates[i];
                    if (startOfRange == DateTime.MinValue)
                    {
                        startOfRange = item;
                    }
                    else//not the first
                    {
                        Debug.Assert(item>=prevItem , "Expect ascending sequence");
                        if (prevItem.AddDays(1) == item)
                        {//contiguous,  continue
                        }
                        else
                        {//save the range
                            listOfRanges.Add(new DateTimeRange(startOfRange,prevItem));
                            startOfRange = item;
                        }
                    }
                    if(i==listOfDates.Count-1)
                    {//The last, close current range
                        listOfRanges.Add(new DateTimeRange(startOfRange, item));
                        break;
                    }
                    prevItem = item;
                }
                Debug.Assert(listOfRanges.Count > 0);
                return listOfRanges;
            }
            /// <summary>
            /// Parse string to Datae Range- possible formats
            /// "BETWEEN a and B"
            /// "from a to B"
            /// </summary>
            /// <param name="?"></param>
            /// <returns></returns>
            public static DateTimeRange Parse(string sRange)
            {
                string patternFromTo = @"from\s(.*)\sto\s(.*)";//from\s(\S*)\sto\s(\S*)";//from\s(\w)\sto\s(\w)"; from\s(.*?)\sto\s(.*?)
                string sFrom = "", sTo = "";
                Match m = Regex.Match(sRange, patternFromTo, RegexOptions.IgnoreCase);
                //bool bValid = false;
                DateTimeRange dtRange = new DateTimeRange();
                if (m.Success == true)
                {
                    sFrom = m.Groups[1].Captures[0].ToString();
                    sTo = m.Groups[2].Captures[0].ToString();
                }
                else
                {
                    patternFromTo = @"BETWEEN\s(.*)\sAND\s(.*)";
                    m = Regex.Match(sRange, patternFromTo, RegexOptions.IgnoreCase);
                    if (m.Success == true)
                    {
                        sFrom = m.Groups[1].Captures[0].ToString();
                        sTo = m.Groups[2].Captures[0].ToString();
                    }
                }
                if (m.Success == true)
                {
                    //try
                    //{
                    DateTime dtFrom = DateTime.Parse(sFrom);
                    DateTime dtTo = DateTime.Parse(sTo);
                    //}
                    //catch (Exception exc)
                    //{
                    //    Debug.Assert(false,exc.ToString());
                    //    throw;
                    //}
                    //  bValid = true;
                    //times are 0 or not 0
                    if (dtTo.TimeOfDay == (new TimeSpan(0)))
                        dtTo = DateTimeHelper.DateEndOfDay(dtTo);
                    dtRange = new DateTimeRange(dtFrom, dtTo, dtFrom.TimeOfDay, dtTo.TimeOfDay);
                }
                return dtRange;
            }
#endregion  //Static Public methods

        }//end struct DateTimeRange
