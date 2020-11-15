using System;
using System.Diagnostics;
//using Microsoft.VisualBasic.CompilerServices;
using CommonDotNetHelpers.Strings;

namespace CommonDotNetHelpers.Data.Time
{
    public static class DateTimeHelper
    {
        //DateType  has been deprecated as of Visual Basic 2005.
        public static DateTime DateAndTime(DateTime dateValue, DateTime timeValue)
        {
            DateTime time2;
            time2 = new DateTime(dateValue.Year, dateValue.Month, dateValue.Day, timeValue.Hour, timeValue.Minute, timeValue.Second);
            return time2;
        }
#if NET461 //TODO make Core compatible
        public static DateTime TimeToDateTime(object time)
        {
            DateTime dt;// = new DateTime();
            if (time is TimeSpan)
            {
                TimeSpan ts = (TimeSpan)time;
                //dt = new DateTime();
                dt = TimeToDateTime(ts);//dt.Add(ts);
            }
            else
                dt = DateType.FromObject(time);
            return dt;
        }
#endif //NET461 //TODO make Core compatible
        public static DateTime TimeToDateTime(TimeSpan ts)
        {
            DateTime dt=new DateTime();
            dt = dt.Add(ts);
            return dt;
        }

        public static DateTime TimeOfDay(DateTime dateValue)
        {
            TimeSpan ts = dateValue.TimeOfDay;
            return TimeToDateTime(ts);
        }


        public static DateTime DateEndOfDay(this DateTime dateValue)
        {
            var dateEndOfDay = dateValue.Date + new TimeSpan(0, 23, 59, 59, 999);
            return dateEndOfDay;
        }
        [Obsolete(" use dateValue.Date instead")]
#if NET461 //TODO make Core compatible
		public static DateTime DateStartOfDay(this DateTime dateValue)
        {
            return DateTimeHelper.DateAndTime(dateValue, DateType.FromString("00:00"));
        }
#endif //NET461 //TODO make Core compatible
		public static DateTime StartOfMonth(this DateTime dateValue)
		{
			return new DateTime(dateValue.Year,dateValue.Month,1,0,0,0);
		}
		public static DateTime StartOfNextMonth(this DateTime dateValue)
		{
			return StartOfMonth(dateValue).AddMonths(1);
		}
#if NET461 //TODO make Core compatible

        public static string FormatDateTimeSQL(this DateTime value, bool includeMillisec=false)
        {
            var format = "yyyy-MM-dd HH:mm:ss";
            if (includeMillisec) format += ".fff";
            return DataHelper.Quoted(Microsoft.VisualBasic.Strings.Format(value, format));
        }

        public static string ToShortDateTime(this DateTime value)
        {
            return Microsoft.VisualBasic.Strings.Format(value, "yyyy-MM-dd HH:mm:ss");
        }
        public static string FormatDateTimeSQL(DateTime dateValue, DateTime timeValue)
        {
            return DataHelper.Quoted(Microsoft.VisualBasic.Strings.Format(dateValue, "yyyy-MM-dd ") + Microsoft.VisualBasic.Strings.Format(timeValue, "HH:mm:ss"));
        }
        public static string TimeConvertSQL(DateTime Value)
        {
            return (" CONVERT(VARCHAR, " + DataHelper.Quoted(Microsoft.VisualBasic.Strings.Format(Value, "HH:mm:ss")) + ",108)");
        }
        public static string DateConvertSQL(DateTime Value)
        {
            //        'Using Convert is not good because indexes are not used see http://www.databasejournal.com/features/mssql/print.php/10894_2209321_3.
            return (" CONVERT(DATETIME, " + DateTimeHelper.DateFormatSQL(Value) + " , 102) ");
        }

        public static string DateFormatSQL(DateTime Value)
        {
            return DataHelper.Quoted(Microsoft.VisualBasic.Strings.Format(Value, "yyyy-MM-dd"));
        }
        public static string SQLBetweenDates(string Field, DateTime FromDate, DateTime ToDate)
        {
            string str = "[" + Field + "] " + SQLBetweenDates(FromDate, ToDate);
            return str;
        }
        public static string SQLBetweenDates(DateTime FromDate, DateTime ToDate)
        {
            string str = "BETWEEN  " + DateConvertSQL(FromDate) + " AND " + FormatDateTimeSQL(DateEndOfDay(ToDate));
            return str;
        }

        public static DateTime YYYYMMDDToDate(long day)
        {
            DateTime time3;
            object obj1 = day.ToString();
            time3 = new DateTime(IntegerType.FromString(Microsoft.VisualBasic.Strings.Left(StringType.FromObject(obj1), 4)), IntegerType.FromString(Microsoft.VisualBasic.Strings.Mid(StringType.FromObject(obj1), 5, 2)), IntegerType.FromString(Microsoft.VisualBasic.Strings.Mid(StringType.FromObject(obj1), 7, 2)));
            return time3;
        }
        public static DateTime HHMMSSToTime(long time)
        {
            DateTime time3;
            object obj1 = time.ToString();
            time3 = new DateTime(0x6d9, 1, 1, IntegerType.FromString(Microsoft.VisualBasic.Strings.Left(StringType.FromObject(obj1), 2)), IntegerType.FromString(Microsoft.VisualBasic.Strings.Mid(StringType.FromObject(obj1), 3, 2)), IntegerType.FromString(Microsoft.VisualBasic.Strings.Mid(StringType.FromObject(obj1), 5, 2)));
            return time3;
        }
#endif //NET461 //TODO make Core compatible

        /// <summary>
        /// DateTime string support relative date format e.g. "-1day 20:00" means yesterday at 20:00
        /// </summary>
        /// <param name="sDateTime"></param>
        /// <returns></returns>
        public static DateTime ParseRelativeDateTime(string sDateTime)
        {
            DateTime retDateTime;
            if (DateTime.TryParse(sDateTime, out retDateTime) != true)
            {//support relative date format e.g. "-1day 20:00" means yesterday at 20:00
                string[] sDateTimeParts = sDateTime.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                if (sDateTimeParts.Length != 2)
                {
                    throw new ArgumentException(String.Format("DateTime format {0} is not recognized ", sDateTime));
                }
                string sRelativeDays = sDateTimeParts[0].Trim();

                string sUnitKey = "day";// //todo support month and other
                if (!sRelativeDays.ToLower().EndsWith(sUnitKey))
                {
                    throw new ArgumentException(String.Format("DateTime format {0} is not recognized ", sDateTime));
                }
                sRelativeDays = sRelativeDays.Replace(sUnitKey, "");
                int nDays;
                if (int.TryParse(sRelativeDays, out nDays) != true)
                {
                    throw new ArgumentException(String.Format("Unable to parse relative days {1} from DateTime {0}  ", sDateTime, sRelativeDays));
                }
                TimeSpan tsDays = new TimeSpan(nDays, 0, 0, 0);
                retDateTime = DateTime.Today.Add(tsDays);
                Debug.Assert(retDateTime.Minute == 0);
                Debug.Assert(retDateTime.Hour == 0);
                Debug.Assert(retDateTime.Second == 0);

                string sTime = sDateTimeParts[1].Trim();
                DateTime timeOfDay;
                if (DateTime.TryParse(sTime, out timeOfDay) != true)
                {
                    throw new ArgumentException(String.Format("Unable to parse time {1} from DateTime {0} ", sDateTime, sTime));
                }
                retDateTime = new DateTime(retDateTime.Year, retDateTime.Month, retDateTime.Day, timeOfDay.Hour, timeOfDay.Minute, timeOfDay.Second, timeOfDay.Millisecond);
            }
            return retDateTime;
        }
        public static bool IsDayOfWeekInRange(DayOfWeek dayOfWeek,DateTime dateFrom, DateTime dateTo, bool bExcludeEndDate)
        {
            DateTime tempDate = dateFrom;
//            if(dateFrom> dateTo){ throw new ArgumentException(String.Format("dateFrom {0} must be less or equal than dateTo {1}",dateFrom,  dateTo)};
            DateTime dateEnd=bExcludeEndDate ? dateTo.AddDays(-1): dateTo; 
            while (tempDate<=dateEnd)
            {
                if(tempDate.DayOfWeek == dayOfWeek)
                    return true;
                tempDate = tempDate.AddDays(1);
            }
            return false;
        }
        public static DateTime Min(DateTime t1, DateTime t2)
        {
            if (DateTime.Compare(t1, t2) > 0)
            {
                return t2;
            }
            return t1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        /// <example>                startDate = DateTimeHelper.Max(SearchStartDate.Value,DateTime.Today);
        ///</example>
        public static DateTime Max(DateTime t1, DateTime t2)
        {
            if (DateTime.Compare(t1, t2) < 0)
            {
                return t2;
            }
            return t1;
        }
		public static int TotalYearsAge(DateTime dob)
		{

			return TotalYearsAge(dob, DateTime.Now);
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dob"></param>
        /// <param name="fromDate">Can be specified, e.g  the time of first or last flight</param>
        /// <returns></returns>
		public static int TotalYearsAge(DateTime dob, DateTime fromDate)
        {
			int intAge = fromDate.Subtract(dob).Days / 365;
            return intAge;
        }
        /// <summary>
        /// E.g "DD, d MM, yy" to ,"dddd, d MMMM, yyyy"
        /// </summary>
        /// <param name="datePickerFormat"></param>
        /// <returns></returns>
        /// <remarks>
        /// Idea to replace from http://stackoverflow.com/questions/8531247/jquery-datepickers-dateformat-how-to-integrate-with-net-current-culture-date
        ///From http://docs.jquery.com/UI/Datepicker/$.datepicker.formatDate to http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx
        ///Format a date into a string value with a specified format.
        ///d - day of month (no leading zero)	---.Net the same
        ///dd - day of month (two digit)		---.Net the same
        ///D - day name short					---.Net "ddd"
        ///DD - day name long					---.Net "dddd"
        ///m - month of year (no leading zero)	---.Net "M"
        ///mm - month of year (two digit)		---.Net "MM"
        ///M - month name short					---.Net "MMM"
        ///MM - month name long					---.Net "MMMM"
        ///y - year (two digit)					---.Net "yy"
        ///yy - year (four digit)				---.Net "yyyy"
        /// </remarks>
   
		public static string JQueryDatePickerFormatToDotNetDateFormat(string datePickerFormat)
		{

			string sRet = datePickerFormat.ReplaceWholeWord("DD", "dddd").ReplaceWholeWord("D", "ddd");
			sRet = sRet.ReplaceWholeWord("M", "MMM").ReplaceWholeWord("MM", "MMMM").ReplaceWholeWord("m", "M").ReplaceWholeWord("mm", "MM");//order is important
			sRet = sRet.ReplaceWholeWord("yy", "yyyy").ReplaceWholeWord("y", "yy");//order is important
			return sRet;
		}

        public static DateTime ConvertServerTimeToTimeZoneTime(DateTime inputDateTime, string zoneId, bool print=false)
        {
            //TimeZone id should be valid from http://stackoverflow.com/questions/7908343/list-of-timezone-ids-for-use-with-findtimezonebyid-in-c/7908482#7908482
            TimeZoneInfo timeInfo = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(inputDateTime);// inputDateTime.ToUniversalTime();
            DateTime targetTimeZoneTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeInfo);
            if (print) //for debug
            {
                Trace.WriteLine("inputDateTime: " + inputDateTime);
                Trace.WriteLine("utcTime: " + utcTime);
                Trace.WriteLine("Zone" + timeInfo + " targetTimeZoneTime: " + targetTimeZoneTime);
            }
            return targetTimeZoneTime;
        }
        #region  next day of week functions

        //from http://forums.asp.net/p/873363/883549.aspx       
        /// <summary>
        /// Gets the next occurence of future day.
        /// </summary>
        /// <param name="value">DateTime value to start with.</param>
        /// <param name="dayOfWeek">Next Day of week to find.</param>
        public static DateTime GetNextOccurenceOfDay(DateTime value, DayOfWeek dayOfWeek)
        {
            return GetNextOccurenceOfDay(value, dayOfWeek, false);
        }
        public static DateTime GetNextOccurenceOfDay(DateTime value, DayOfWeek dayOfWeek,bool bIncludeStartDay)
        {
            //TODO add ClosestDay that should start formprovided day
            int daysToAdd = dayOfWeek - value.DayOfWeek;
            int nMinDifference=bIncludeStartDay ? 0:1;
            if (daysToAdd < nMinDifference)
            {
                daysToAdd += 7;
            }
            return value.AddDays(daysToAdd);

            //DateTime tempDate = value.AddDays(1);
            //while (tempDate.DayOfWeek != dayOfWeek)
            //{
            //    tempDate = tempDate.AddDays(1);
            //}
            //return tempDate;
        }
        /// <summary>
        /// Gets the next occurence of specified  day.
        /// </summary>
        /// <param name="value">DateTime value to start with.</param>
        public static DateTime NextMonday(DateTime value)
        {
            return GetNextOccurenceOfDay(value, DayOfWeek.Monday);
        }
        /// <summary>
        /// Gets the next occurence of specified  day.
        /// </summary>
        /// <param name="value">DateTime value to start with.</param>
        public static DateTime NextWednesday(DateTime value)
        {
            return GetNextOccurenceOfDay(value, DayOfWeek.Wednesday);
        }
        /// <summary>
        /// Gets the next occurence of specified  day.
        /// </summary>
        /// <param name="value">DateTime value to start with.</param>
        public static DateTime NextFriday(DateTime value)
        {
            return GetNextOccurenceOfDay(value, DayOfWeek.Friday);
        }
        /// <summary>
        /// Gets the next occurence of specified  day.
        /// </summary>
        /// <param name="value">DateTime value to start with.</param>
        public static DateTime NextSunday(DateTime value)
        {
            return GetNextOccurenceOfDay(value, DayOfWeek.Sunday);
        }
        /// <summary>
        /// Gets the next occurence of specified  day.
        /// </summary>
        /// <param name="value">DateTime value to start with.</param>
        public static DateTime NextSaturday(DateTime value)
        {
            return GetNextOccurenceOfDay(value, DayOfWeek.Saturday);
        }
        public static DateTime NextTuesday(DateTime value)
        {
            return GetNextOccurenceOfDay(value, DayOfWeek.Tuesday);
        }

        public static DateTime MondayThisOrNext(DateTime value)
        {
            return GetNextOccurenceOfDay(value, DayOfWeek.Monday, true);
        }
        public static DateTime TuesdayThisOrNext(DateTime value)
        {
            return GetNextOccurenceOfDay(value, DayOfWeek.Tuesday, true);
        }
        /// <summary>
        /// Gets the next occurence of specified  day.
        /// </summary>
        /// <param name="value">DateTime value to start with.</param>
        public static DateTime WednesdayThisOrNext(DateTime value)
        {
            return GetNextOccurenceOfDay(value, DayOfWeek.Wednesday, true);
        }
        public static DateTime ThursdayThisOrNext(DateTime value)
        {
            return GetNextOccurenceOfDay(value, DayOfWeek.Thursday, true);
        }
        /// <summary>
        /// Gets the next occurence of specified  day.
        /// </summary>
        /// <param name="value">DateTime value to start with.</param>
        public static DateTime FridayThisOrNext(DateTime value)
        {
            return GetNextOccurenceOfDay(value, DayOfWeek.Friday, true);
        }
        /// <summary>
        /// Gets the next occurence of specified  day.
        /// </summary>
        /// <param name="value">DateTime value to start with.</param>
        public static DateTime SaturdayThisOrNext(DateTime value)
        {
            return GetNextOccurenceOfDay(value, DayOfWeek.Saturday, true);
        }
        /// <summary>
        /// Gets the next occurence of specified  day.
        /// </summary>
        /// <param name="value">DateTime value to start with.</param>
        public static DateTime SundayThisOrNext(DateTime value)
        {
            return GetNextOccurenceOfDay(value, DayOfWeek.Sunday, true);
        }


#endregion  //next/closest day of week functions



    }
}
