using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    public class GdDisplayNameAttribute : DisplayNameAttribute
    {
        public GdDisplayNameAttribute(string displayNameKey)
        : base(displayNameKey)
        {
        }

        public override string DisplayName
        {
            get
            {
                string defaultCulture = Thread.CurrentThread.CurrentUICulture.Name;
                string connStr = ConfigurationManager.ConnectionStrings["ApplicationContext"].ConnectionString;
                List<TextTranslations> translations = new List<TextTranslations>();
                string s = "";

                if (HttpContext.Current?.Session != null && HttpContext.Current?.Session["caTenant"] != null)
                {
                    caTenantBase tenant = (caTenantBase)HttpContext.Current.Session["caTenant"];
                    const string sql = "select Culture, Name, Value from dbo.TextTranslations where Name = @name;";
                    using (var con = new SqlConnection(connStr))
                    {
                        var cmd = new SqlCommand(sql, con);
                        cmd.Parameters.AddWithValue("@name", base.DisplayName);

                        try
                        {
                            con.Open();
                        }
                        catch (Exception)
                        {
                            return s;
                        }
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                translations.Add(new TextTranslations
                                {
                                    Name = reader["Name"].ToString(),
                                    Value = reader["Value"].ToString(),
                                    Culture = reader["Culture"].ToString()
                                });
                            }
                        }
                    }

                    string res = translations?.FirstOrDefault(x => x.Culture == tenant.TenantCulture)?.Value;
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        s = res;
                    }
                }

                if ((!string.IsNullOrWhiteSpace(defaultCulture)) && string.IsNullOrWhiteSpace(s))
                {
                    string res = translations?.FirstOrDefault(x => x.Culture == defaultCulture)?.Value;
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        s = translations?.FirstOrDefault(x => x.Culture == defaultCulture)?.Value;
                    }

                }

                if (string.IsNullOrWhiteSpace(s))
                {
                    s = base.DisplayNameValue;
                }

                return s;
            }
        }
    }

    public class GdPhoneAttribute : ValidationAttribute
    {
        private Regex _regex = new Regex("^\\s*\\+?\\s*([0-9][\\s-]*){9,}$");

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success;
            }

            if (_regex.IsMatch(value.ToString()))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(string.Format("Please enter a valid {0} number", validationContext.DisplayName));
        }
    }
}