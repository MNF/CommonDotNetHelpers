using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

#if NET461 
using System.Data;
using System.Web;
using System.Web.Security;
using Microsoft.VisualBasic.CompilerServices;
#else
//using Microsoft.AspNetCore.Http; 
#endif // NET461

using CommonDotNetHelpers.Collections;
using CommonDotNetHelpers.Data;

namespace CommonDotNetHelpers.Diagnostics
{
    //do not declare static, because it has derived DebugOutputHelper
    public static class TraceOutputHelper
    {
        // Methods
        //public TraceOutputHelper()
        //{
        //}

#if NET461
        #region Dataset
        public static string DatasetAsString(DataSet ds, string sComment /* = "" */)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(sComment))
            {
                sb.AppendLine(sComment);
            }

            if (ds == null)
            {
                sb.AppendLine("Null DataSet");
            }
            else
            {
                sb.AppendLine("Tables Count: " + ds.Tables.Count.ToString());
                foreach (DataTable table1 in ds.Tables)
                {
                    sb.AppendLine(TableAsString(table1, ""));
                }
            }

            return sb.ToString();
        }
        public static string TableAsString(DataTable tbl, string sComment /* = "" */)
        {
            var sb = new StringBuilder();
            if (!String.IsNullOrEmpty(sComment))
            {
                sb.AppendLine(sComment);
            }
            if (tbl == null)
            {
                sb.AppendLine("Table is null");
                return sb.ToString();
            }
            sb.Append(String.Format("Table: '{0}' with {1} rows ", tbl.TableName, tbl.Rows.Count));
            foreach (DataRow row1 in tbl.Rows)
            {
                sb.AppendLine(RowAsString(row1, ""));
            }
            return sb.ToString();
        }

        #endregion //Dataset
        //		'Returns false if can't be coverted to string
        //		'sValue is returned as string  if can be converted to string or Null if DBNull 
        public static bool ColumnValueToString(DataRow row, DataColumn myColumn, ref string sValue)
        {
            return ColumnValueToString(myColumn, row[myColumn], ref sValue);
        }
        //		'Returns false if can't be coverted to string
        //		'sValue is returned as string  if can be converted to string or Null if DBNull 
        public static bool ColumnValueToString(DataRowView row, DataColumn myColumn, ref string sValue)
        {
            return ColumnValueToString(myColumn, row[myColumn.Ordinal], ref sValue);
        }

        private static bool ColumnValueToString(DataColumn myColumn, Object oColumnValue, ref string sValue)
        {
            bool bRet = false;
            if (IsPrintableType(myColumn.DataType))
            {
                bRet = true;
            }
            else if ((myColumn.DataType == Type.GetType("System.Object")) && IsPrintableType(oColumnValue.GetType()))
            {
                sValue = StringType.FromObject(oColumnValue);
                bRet = true;
            }
            if (bRet)
            {
                sValue = StringType.FromObject(DataHelper.Nz((oColumnValue), "Null"));
            }
            return bRet;
        }
#endif // NET461

        //
        /// <summary>
        /// Assumes that all elemets of ArrayList messages are strings.
        /// If not sure, call ArrayListToString
        /// See also very similar StringArrayAsString TODO: merge
        ///Seealso CollectionsHelper.ToString<T>
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="sComment"></param>
        /// <returns></returns>
        public static string StringArrayToString(ArrayList messages, string sComment)
        {
            string sRet = "";
            if (!String.IsNullOrEmpty(sComment))
            {
                sRet = sRet + sComment;
            }
            foreach (string error in messages)
            {
                sRet += error + Environment.NewLine;
            }
            return sRet;
        }


        /// <summary>
        /// If you sure, that all elements are string, use List<string> or for legacy code StringArrayToString 
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="sComment"></param>
        /// <returns></returns>
        public static string ArrayListToString(ArrayList messages, string sComment)
        {
            //TODO confirm that only strings are stored and convert to List<string>
            StringBuilder sb = new StringBuilder(sComment);
            if (messages != null)
            {
                foreach (object msg in messages)
                {
                    if (msg != null)
                    {
                        sb.AppendLine(msg.ToString());
                    }
                }
            }
            return sb.ToString();
        }

        //from http://www.code-magazine.com/articleprint.aspx?quickid=0712052&printmode=true
        public static string PropertiesToString(object source)
        {
            StringBuilder sb = new StringBuilder();
            Type t = source.GetType();
            foreach (PropertyInfo pi in t.GetProperties())
            {
                object o = pi.GetValue(source, null);
                sb.Append(pi.Name + ":");
                sb.AppendLine((o == null) ? "null" : o.ToString());
            }
            sb.Append('\n');
            return (sb.ToString());
        }
    
//     StackTrace .NetCore   Currently there is no workaround, but we are working on it.Please check back.

        public static string FindCallerFunction()
        {
            return FindCallerFunction(0);//exclude itself
        }
        public static string FindCallerFunction(int nSkip)
        {
            //        'Navigate through stack skipping system functions , ErrorReport  and TracedLine methods
            //        'Smarter than System.Reflection.MethodInfo.GetCurrentMethod()
            string sRet = "";
#if NET461
            StackTrace trace1 = new StackTrace();

            int num2 = trace1.FrameCount;
            for (int num1 = 1; num1 <= num2; num1++)
            {
                MethodBase base1 = trace1.GetFrame(num1).GetMethod();
                bool bSkip = false;
                string sReflectedType = base1.ReflectedType.ToString();
                switch (base1.Name)
                {
                    case "TracedLine":
                    case "LineWithTrace":
                    case "ShowException":
                    case "TraceWithTime":
                    case "FindCallerFunction":
                        bSkip = true;
                        break;
                    default:
                        //Exclude known classes, that we don't want to show
                        if (sReflectedType.StartsWith("System.Windows.Forms"))
                        {
                            bSkip = true;
                        }
                        else if (sReflectedType.StartsWith("ControlNativeWindow"))
                        {
                            bSkip = true;
                        }
                        else if (sReflectedType.StartsWith("ThreadContext"))
                        {
                            bSkip = true;
                        }
                        //else if (sReflectedType.EndsWith("ErrorReportForm"))
                        //{
                        //    bSkip = true;
                        //}
                        //else if (sReflectedType.EndsWith("ErrorForm"))
                        //{
                        //    bSkip = true;
                        //}
                        break;
                }
                if (!bSkip)
                {
                    if (nSkip <= 0)
                    {
                        sRet = sReflectedType + "." + base1.Name;
                        break;//FOR							return sRet;
                    }
                    else
                    {
                        nSkip--;
                    }
                }
            }//for 
#else
           string multiLine= Environment.StackTrace; 
            var lines = multiLine.Split('\n');
            sRet = "FindCallerFunction is not implemented in .Net Core " + lines?[0];
#endif // NET461
            return sRet;
        }
#if NET461
#endif // NET461
        public static bool IsPrintableType(Type DataType)
        {
            //        'Todo add other types
            if ((DataType == Type.GetType("System.String")) || (DataType == Type.GetType("System.Decimal")) ||
                (DataType == Type.GetType("System.Int32")) || (DataType == Type.GetType("System.Int64")) ||
                (DataType == Type.GetType("System.Double")) || (DataType == Type.GetType("System.DateTime"))
                || (DataType == Type.GetType("System.Boolean"))
                )
            {
                return true;
            }
            return false;
        }
        public static string WriteTraceLineWithTrace(string format, params object[] arg)
        {
            var s = LineWithTrace(format, arg);
            Trace.WriteLine(s);
            return s;
        }
        /// <summary>
        /// Note that the method generates string, but does not write to the Debug Window.
        /// You may want want to use DebugOutputHelper.TracedLine
        /// </summary>
        /// <param name="sComment"></param>
        /// <returns></returns>
        public static string LineWithTrace(string sComment)
        {
            return (FindCallerFunction() + ":" + sComment);
        }
        public static string LineWithTrace(string format, params object[] arg)
        {
            return (FindCallerFunction() + ":" + string.Format(format, arg));
        }

        public static string TraceWithTime(string sComment)
        {
            return (DateTime.Now.ToString() + " :" + LineWithTrace(sComment));
        }


#if NET461
        public static string ChildrenAsString(System.Web.UI.Control container, string sComment /* = "" */)
        {
            string sRet = "";
            if (!String.IsNullOrEmpty(sComment))
            {
                sRet = sRet + sComment + "\r\n";
            }
            foreach (System.Web.UI.Control control1 in container.Controls)
            {
                sRet = sRet + ("ID= " + control1.ID + "(" + control1.GetType().ToString()) + ")\r\n";
            }
            //	Debug.WriteLine(sRet);
            return sRet;
        }
        public static string RowAsString(DataRow row, string sComment /* = "" */)
        {
            string sRet = "";
            if (!String.IsNullOrEmpty(sComment))
            {
                sRet = sRet + sComment;
            }
            if (row != null && row.Table != null && row.RowState != DataRowState.Deleted)
            {
                foreach (DataColumn column1 in row.Table.Columns)
                {
                    sRet = sRet + ColumnWithValueAsString(column1, row[column1]);
                }
            }
            return sRet;
        }
        public static string RowAsString(DataRowView row, string sComment /* = "" */)
        {
            string sRet = "";
            if (!String.IsNullOrEmpty(sComment))
            {
                sRet = sRet + sComment;
            }
            if (row != null)//3/8/2005
            {
                DataTable tbl = row.DataView.Table;
                if (tbl != null)//8/4/2005
                {
                    foreach (DataColumn column1 in tbl.Columns)
                    {
                        sRet = sRet + ColumnWithValueAsString(column1, row[column1.Ordinal]);
                    }
                }
                else Debug.Assert(false, "row.DataView.Table cannot be null");
            }
            else Debug.Assert(false, "row cannot be null");
            Debug.WriteLine(sRet);
            return sRet;
        }
        private static string ColumnWithValueAsString(DataColumn column1, Object oColumnValue)
        {
            string sRet = "";
            string[] textArray1 = new string[5] { sRet, column1.ColumnName, "(", column1.DataType.ToString(), ")" };
            sRet = string.Concat(textArray1);
            string sValue = "";
            if (ColumnValueToString(column1, oColumnValue, ref sValue))
            {
                sValue = String.Format(" Value={0}\r\n", sValue);
            }
            else
            {
                sValue = " Value not printable \r\n";
            }
            return sRet + sValue;
        }

#endif // NET461

#if !NETSTANDARD1_6
        public static string NotEmptyNameValueCollectionAsString(NameValueCollection coll, string sComment /* = "" */)
        {
            if (CollectionsHelper.IsNullOrEmpty(coll)) return "";
            return NameValueCollectionAsString(coll, sComment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coll">e.g ServerVariables, QueryString etc</param>
        /// <param name="sComment"></param>
        /// <returns></returns>
        public static string NameValueCollectionAsString(NameValueCollection coll, string sComment /* = "" */)
        {
            string sRet = "";
            if (!String.IsNullOrEmpty(sComment))
            {
                sRet += sComment + Environment.NewLine;
            }
            int loop1, loop2;

            // Get names of all keys into a string array. 
            String[] arr1 = coll.AllKeys;
            sRet += "Length= " + arr1.Length.ToString() + Environment.NewLine;
            for (loop1 = 0; loop1 < arr1.Length; loop1++)
            {
                sRet += ("Key: " + arr1[loop1]);//+Environment.NewLine;
                String[] arr2 = coll.GetValues(arr1[loop1]);
                for (loop2 = 0; loop2 < arr2.Length; loop2++)
                {
                    sRet += (" Value " + loop2 + ": " + arr2[loop2]) + Environment.NewLine;
                }
            }
            return sRet;
        }
#endif //!NETSTANDARD1_6

        /// <summary>
        /// Empty dictionary returns ""
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="sComment"></param>
        /// <returns></returns>
        public static string NotEmptyDictionaryAsString(IDictionary dict, string sComment /* = "" */)
        {
            if (dict.IsNullOrEmpty()) return "";
            return DictionaryAsString(dict, sComment);
        }
        /// <summary>
        /// Dictionaries as string.(including Hashtable
        /// </summary>
        /// <param name="dict">The dict.</param>
        /// <param name="sComment">The s comment.</param>
        /// <returns></returns>
		public static string DictionaryAsString(this IDictionary dict, string sComment /* = "" */)
        {
            string sRet = "";
            if (!String.IsNullOrEmpty(sComment))
            {
                sRet += sComment + Environment.NewLine;
            }
            if (dict == null)
            {
                sRet += ("Dictionary is null: ") + Environment.NewLine;
            }
            else
            {
                string sEntry = "";
                sRet += String.Format("Dictionary Count: {0}\n", dict.Count);
                foreach (DictionaryEntry entry1 in dict)
                {
                    if (entry1.Key != null)
                    {
                        sEntry += String.Format("{0} ({1}): ", entry1.Key, entry1.Key.GetType().ToString());
                    }
                    if (entry1.Value != null)
                    {
                        sEntry += entry1.Value.ToString();
                    }
                    sEntry += " \r\n";
                }
                sRet += (sEntry);
            }
            return sRet;
        }
#if NET461

        public static string DictionaryAsString(StringDictionary dict, string sComment /* = "" */)
        {
            string sRet = "";
            if (!String.IsNullOrEmpty(sComment))
            {
                sRet += sComment + Environment.NewLine;
            }
            if (dict == null)
            {
                sRet += ("Dictionary is null: ") + Environment.NewLine;
            }
            else
            {
                string sEntry = "";
                sRet += String.Format("Dictionary Count: {0}\n", dict.Count);
                foreach (DictionaryEntry entry1 in dict)
                {
                    sEntry += String.Format("{0} ({1}): {2} \r\n", entry1.Key, entry1.Key.GetType().ToString(), entry1.Value.ToString());
                }
                sRet += (sEntry);
            }
            return sRet;
        }
#endif // NET461
        //See also very similar StringArrayToString TODO: merge
        public static string StringArrayAsString(string[] asStrings, string sComment)
        {
            string sRet = "";
            if (!String.IsNullOrEmpty(sComment))
            {
                sRet += sComment + Environment.NewLine;
            }
            if ((asStrings == null) || (asStrings.Length == 0))
            {
                sRet += ("Array is empty.") + Environment.NewLine;
            }
            else
            {
                sRet += "Array Count: " +asStrings.Length + Environment.NewLine;
                foreach (string sEntry in asStrings)
                {
                    sRet += (sEntry) + Environment.NewLine;
                }
            }
            return sRet;
        }
        public static string TracedTimeSpanAsString(DateTime timeStart, string sComment)
        {
            return TimeSpanAsString(timeStart, FindCallerFunction(1) + ":" + sComment);
        }
        public static string TimeSpanAsString(DateTime timeStart, string sComment)
        {
            string sRet = "";
            if (!String.IsNullOrEmpty(sComment))
            {
                sRet += sComment + " ";
            }
            TimeSpan span1 = DateTime.Now.Subtract(timeStart);
            sRet += span1.ToString();
            return sRet;
        }
        public static string CurrentDomainPaths()
        {
#if NET461
            return Environment.Version.ToString() + "\r\n" + Assembly.GetExecutingAssembly().CodeBase + "\r\n" +
                "ApplicationBase: " + AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\r\n" +
                "PrivateBinPath: " + AppDomain.CurrentDomain.SetupInformation.PrivateBinPath + "\r\n" +
                "PrivateBinProbe: " + AppDomain.CurrentDomain.SetupInformation.PrivateBinPathProbe + "\r\n" +
                Directory.GetCurrentDirectory() + "\r\n" + AppDomain.CurrentDomain.FriendlyName + "\r\n";

#else
        return    TraceOutputHelper.LineWithTrace("Supported only NETStandard 2.0+  ");
#endif // NET461
        }
#if NET461

        //What Sets IsAuthenticated=True http://www.dotnet247.com/247reference/msgs/22/114539.aspx 
        public static string FormsAuthenticationAsString(string sComment)
        {
            StringBuilder sb = new StringBuilder();
            if (!String.IsNullOrEmpty(sComment))
            {
                sb.Append(sComment + " ");
            }
            sb.Append("System.Web.Security.FormsAuthentication:\n");
            sb.Append(String.Format("CookieDomain:{0}\n", System.Web.Security.FormsAuthentication.CookieDomain));
            sb.Append(String.Format("CookieMode:{0}\n", System.Web.Security.FormsAuthentication.CookieMode.ToString()));
            sb.Append(String.Format("CookiesSupported:{0}\n", System.Web.Security.FormsAuthentication.CookiesSupported.ToString()));
            sb.Append(String.Format("DefaultUrl:{0}\n", System.Web.Security.FormsAuthentication.DefaultUrl.ToString()));
            sb.Append(String.Format("EnableCrossAppRedirects:{0}\n", System.Web.Security.FormsAuthentication.EnableCrossAppRedirects.ToString()));
            sb.Append(String.Format("FormsCookieName:{0}\n", System.Web.Security.FormsAuthentication.FormsCookieName.ToString()));
            sb.Append(String.Format("RequireSSL:{0}\n", System.Web.Security.FormsAuthentication.RequireSSL.ToString()));
            sb.Append(String.Format("FormsCookiePath:{0}\n", System.Web.Security.FormsAuthentication.FormsCookiePath.ToString()));
            sb.Append(String.Format("LoginUrl:{0}\n", System.Web.Security.FormsAuthentication.LoginUrl.ToString()));
            sb.Append(String.Format("SlidingExpiration:{0}\n", System.Web.Security.FormsAuthentication.SlidingExpiration.ToString()));
            return sb.ToString();
        }

        public static string OutputFormsAuthenticationCookie(HttpRequest request)
        {
            string sRet = "";
            string sCookieName = FormsAuthentication.FormsCookieName;
            DebugOutputHelper.TracedLine("  sCookieName=" + sCookieName);
            HttpCookie c = request.Cookies[sCookieName];
            if (c != null) // && c.HasKeys the cookie exists!
            {
                string cookie = "";
                try
                {
                    cookie = HttpContext.Current.Server.UrlDecode(c.Value);
                    FormsAuthenticationTicket fat = FormsAuthentication.Decrypt(cookie);
                    //TODO extend fat 
                    sRet = String.Format("FormsAuthenticationTicket for CookieName={0} is:{1}\n", sCookieName,
                        FormsAuthenticationTicketAsString(fat, ""));
                }
                catch
                {
                    sRet = String.Format("Unable to retrieve FormsAuthenticationTicket from cookie. FormsCookieName={0} cookie={1}\n", sCookieName, cookie);
                }
            }
            else
            {
                sRet = String.Format("No FormsAuthentication Cookie found. FormsCookieName={0}\n", sCookieName);
                sRet += TraceOutputHelper.CookieCollectionAsString(request.Cookies, "all cookies"); ;
            }
            return sRet;
        }

        public static string FormsAuthenticationTicketAsString(FormsAuthenticationTicket ticket, string sComment)
        {
            StringBuilder sb = new StringBuilder();
            if (!String.IsNullOrEmpty(sComment))
            {
                sb.Append(sComment + Environment.NewLine);
            }
            sb.AppendFormat("Name: {0}\n", ticket.Name);
            sb.AppendFormat("CookiePath: {0}\n", ticket.CookiePath);
            sb.AppendFormat("IssueDate: {0}\n", ticket.IssueDate);
            sb.AppendFormat("Expiration: {0}\n", ticket.Expiration);
            sb.AppendFormat("IsPersistent: {0}\n", ticket.IsPersistent);
            sb.AppendFormat("Version: {0}\n", ticket.Version);
            sb.AppendFormat("UserData: {0}\n", ticket.UserData);

            return sb.ToString();
        }
#endif // NET461
        //See also overload for HttpCookieCollection 
        public static string CookieCollectionAsString(System.Net.CookieCollection cookies, string sComment, LoggingOutputLevel level)
        {
            StringBuilder sb = new StringBuilder();
            //            string sRet = "";
            if (!String.IsNullOrEmpty(sComment))
            {
                sb.Append(sComment + Environment.NewLine);
            }
            sb.AppendFormat("Cookies collection. Count={0}\n", cookies.Count.ToString());
            // Print the properties of each cookie.
            foreach (Cookie cookie1 in cookies)
            {
                if (level > LoggingOutputLevel.Brief)
                {
                    sb.AppendFormat("{0} = {1}\n", cookie1.Name, cookie1.Value);
                    sb.AppendFormat("Domain: {0}\n", cookie1.Domain);
                    sb.AppendFormat("Path: {0}\n", cookie1.Path);
                    sb.AppendFormat("Port: {0}\n", cookie1.Port);
                    sb.AppendFormat("Secure: {0}\n", cookie1.Secure);
                    sb.AppendFormat("When issued: {0}\n", cookie1.TimeStamp);
                    sb.AppendFormat("Expires: {0} (expired? {1})", cookie1.Expires, cookie1.Expired);
                    sb.AppendFormat("Don't save: {0}\n", cookie1.Discard);
                    sb.AppendFormat("Comment: {0}\n", cookie1.Comment);
                    sb.AppendFormat("Uri for comments: {0}\n", cookie1.CommentUri);
                    sb.AppendFormat("Version: RFC {0}\n", cookie1.Version == 1 ? "2109" : "2965");
                }
                // Show the string representation of the cookie.
                sb.AppendFormat("String: {0}", cookie1.ToString());
            }
            return sb.ToString();
        }

#if NET461

        public static string CookieCollectionAsString(HttpCookieCollection cookies, string sComment)
        {
            StringBuilder sb = new StringBuilder();
            //            string sRet = "";
            if (!String.IsNullOrEmpty(sComment))
            {
                sb.Append(sComment + Environment.NewLine);
            }
            sb.Append("HttpCookieCollection Count=" + cookies.Count.ToString());

            for (int i = 0; i < cookies.Count; i++)
            {
                sb.Append("Name: " + cookies[i].Name + ",");
                sb.Append("Value: " + cookies[i].Value + ",");
                sb.Append("Expires: " + cookies[i].Expires.ToString() + Environment.NewLine);
            }
            return sb.ToString();
        }
#else
        /* TODO: if needed, move to class dependent on Microsoft.AspNetCore.Http
                public static string CookieCollectionAsString(this IRequestCookieCollection cookies, string sComment=null, LoggingOutputLevel level= LoggingOutputLevel.Brief)
                {
                    StringBuilder sb = new StringBuilder();
                    //            string sRet = "";
                    if (!String.IsNullOrEmpty(sComment))
                    {
                        sb.Append(sComment + Environment.NewLine);
                    }
                    sb.AppendFormat("Cookies collection  Count={0}\n", cookies.Count.ToString());
                    var totalLength = 0;
                    // Print the properties of each cookie.
                    foreach (var cookie in cookies)
                    {
                        totalLength += cookie.Key.Length + cookie.Value.Length;
                        sb.AppendFormat(" {0} :", cookie.Key);
                        if (level > LoggingOutputLevel.Brief)
                        {
                            sb.AppendFormat("{0} \n", cookie.Value);
                        }
                        else
                        {
                            sb.AppendFormat("len(value)= {0})\n", cookie.Value.Length);
                        }
                    }
                    // Show the string representation of the cookie.
                    sb.AppendFormat("Total Cookies length: {0}", totalLength);
                    return sb.ToString();
                }
        */
#endif // NET461
        public static string ListAsString<T>(IList<T> list, string sComment = null)
        {
            StringBuilder sb = new StringBuilder();
            if (!String.IsNullOrEmpty(sComment))
            {
                sb.Append(sComment + Environment.NewLine);
            }
            sb.Append("List Count=" + list.Count.ToString() + Environment.NewLine);
            foreach (T item in list)
            {
                sb.Append(item.ToString() + Environment.NewLine);
            }
            return sb.ToString();
        }

        //Attempt to create 28/3/2005 
        //issue base class doesn't have public indexer, and have to pass NameValueCollection
        // so see NameValueCollectionAsString
        //		public static string NameObjectCollectionAsString(NameValueCollection coll,  string sComment /* = "" */)
        //		{
        //			string sRet = "";
        //			if (!String.IsNullOrEmpty(sComment))
        //			{
        //				sRet +=  sComment+Environment.NewLine ;
        //			}
        //			if (coll == null)
        //			{
        //				sRet +=("Collection is null: ")+Environment.NewLine ;
        //			}
        //			else
        //			{
        //				string sEntry="";
        //				sRet +=("NameObjectCollection Count: " + StringType.FromInteger(coll.Count));
        //				//ms-help://MS.VSCC.2003/MS.MSDNQTR.2003FEB.1033/cpref/html/frlrfsystemcollectionsspecializednameobjectcollectionbaseclasstopic.htm
        //				foreach ( String sKey in coll.Keys )  
        //				{
        //					sEntry =String.Format( "{0}, {1}", sKey, coll[sKey] );
        //					sRet +=(sEntry);
        //				}
        //			}
        //			return sRet;
        //		}
#if WINFORMS_TRACES_REQUIRED //not used at the moment
		public static string TraceString(BindingMemberInfo bInfo, [Optional] string sComment /* = "" */)
		{
			string text2 = "";
			if (!DataHelper.IsNullOrEmpty(sComment))
			{
				text2 = text2 + sComment;
			}
			text2 = text2 + " BindingPath: " + bInfo.BindingPath + "\r\n";
			text2 = text2 + " BindingField: " + bInfo.BindingField + "\r\n";
			text2 = text2 + " BindingMember: " + bInfo.BindingMember + "\r\n";
			//Debug.WriteLine(text2);
			return text2;
		}

		//[Conditional("DEBUG")]
		public static string TraceString(ControlBindingsCollection bindings, [Optional] string sComment /* = "" */)
		{
			string text2 = "";
			string sRet = "";
			if (!DataHelper.IsNullOrEmpty(sComment))
			{
				text2=sComment;
			}
			foreach(Binding binding1 in bindings)
			{
					sRet = "";
					sRet = sRet + " DataSource: " + binding1.DataSource.ToString();
					sRet = sRet + " PropertyName: " + binding1.PropertyName;
					Debug.WriteLine(sRet);
					text2 = text2 + sRet;
					text2 = text2 + TraceString(binding1.BindingMemberInfo, "");
			}
			return text2;
		}
	public static string BindingMemberInfoAsString(System.Windows.Forms.Control container, [Optional] string sComment /* = "" */)
		{
			string text2 = "";
			if (!DataHelper.IsNullOrEmpty(sComment))
			{
				text2=sComment;
			}
				foreach (System.Windows.Forms.Control control1 in container.Controls )
				{
					text2+=TraceString(control1.DataBindings, "");
				}
			return text2;
		}
#endif

    }
}
