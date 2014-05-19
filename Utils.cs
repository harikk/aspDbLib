using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

namespace VICommons
{
    public class Utils
    {
        public static string RemoveTrailingSymbol(string str, string symbol)
        {
            str = str.Trim();
            if (str.Length > symbol.Length)
            {
                if (str.LastIndexOf(symbol) == str.Length - symbol.Length)
                {
                    return str.Substring(0, str.Length - symbol.Length);
                }
            }
            return str;
        }

        public static void FillList(ListControl ctrl, DataTable dataSource, string DataTextField, string DataValueField, bool UseOthersOption = true, string OthersLabel = "Select", string othersValue = "-1")
        {
            ctrl.Items.Clear();
            ctrl.DataTextField = DataTextField;
            ctrl.DataValueField = DataValueField;
            ctrl.DataSource = dataSource;
            ctrl.DataBind();
            if (UseOthersOption)
            {
                ctrl.Items.Insert(0, new ListItem(OthersLabel, "-1"));
            }
        }
    }
}