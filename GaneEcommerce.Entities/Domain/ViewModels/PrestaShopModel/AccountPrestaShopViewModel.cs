using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Ganedata.Core.Entities.Domain.ViewModels
{
	[XmlRoot(ElementName = "id_default_group")]
	public class Id_default_group
	{
		[XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
		public string Href { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	
	[XmlRoot(ElementName = "group")]
	public class Group
	{
		[XmlElement(ElementName = "id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
		public string Href { get; set; }
	}

	[XmlRoot(ElementName = "groups")]
	public class PrestashopGroups
	{
		[XmlElement(ElementName = "group")]
		public Group Group { get; set; }
		[XmlAttribute(AttributeName = "nodeType")]
		public string NodeType { get; set; }
		[XmlAttribute(AttributeName = "api")]
		public string Api { get; set; }
	}

	[XmlRoot(ElementName = "associations")]
	public class AssociationsAccount
	{
		[XmlElement(ElementName = "groups")]
		public PrestashopGroups Groups { get; set; }
	}

	[XmlRoot(ElementName = "customer")]
	public class Customer
	{
		[XmlElement(ElementName = "id")]
		public int Id { get; set; }
		[XmlElement(ElementName = "id_default_group")]
		public Id_default_group Id_default_group { get; set; }
		[XmlElement(ElementName = "id_lang")]
		public Id_lang Id_lang { get; set; }
		[XmlElement(ElementName = "newsletter_date_add")]
		public string Newsletter_date_add { get; set; }
		[XmlElement(ElementName = "ip_registration_newsletter")]
		public string Ip_registration_newsletter { get; set; }
		[XmlElement(ElementName = "last_passwd_gen")]
		public string Last_passwd_gen { get; set; }
		[XmlElement(ElementName = "secure_key")]
		public string Secure_key { get; set; }
		[XmlElement(ElementName = "deleted")]
		public string Deleted { get; set; }
		[XmlElement(ElementName = "passwd")]
		public string Passwd { get; set; }
		[XmlElement(ElementName = "lastname")]
		public string Lastname { get; set; }
		[XmlElement(ElementName = "firstname")]
		public string Firstname { get; set; }
		[XmlElement(ElementName = "email")]
		public string Email { get; set; }
		[XmlElement(ElementName = "id_gender")]
		public string Id_gender { get; set; }
		[XmlElement(ElementName = "birthday")]
		public string Birthday { get; set; }
		[XmlElement(ElementName = "newsletter")]
		public string Newsletter { get; set; }
		[XmlElement(ElementName = "optin")]
		public string Optin { get; set; }
		[XmlElement(ElementName = "website")]
		public string Website { get; set; }
		[XmlElement(ElementName = "company")]
		public string Company { get; set; }
		[XmlElement(ElementName = "siret")]
		public string Siret { get; set; }
		[XmlElement(ElementName = "ape")]
		public string Ape { get; set; }
		[XmlElement(ElementName = "outstanding_allow_amount")]
		public string Outstanding_allow_amount { get; set; }
		[XmlElement(ElementName = "show_public_prices")]
		public string Show_public_prices { get; set; }
		[XmlElement(ElementName = "id_risk")]
		public string Id_risk { get; set; }
		[XmlElement(ElementName = "max_payment_days")]
		public string Max_payment_days { get; set; }
		[XmlElement(ElementName = "active")]
		public string Active { get; set; }
		[XmlElement(ElementName = "note")]
		public string Note { get; set; }
		[XmlElement(ElementName = "is_guest")]
		public string Is_guest { get; set; }
		[XmlElement(ElementName = "id_shop")]
		public string Id_shop { get; set; }
		[XmlElement(ElementName = "id_shop_group")]
		public string Id_shop_group { get; set; }
		[XmlElement(ElementName = "date_add")]
		public string Date_add { get; set; }
		[XmlElement(ElementName = "date_upd")]
		public string Date_upd { get; set; }
		[XmlElement(ElementName = "reset_password_token")]
		public string Reset_password_token { get; set; }
		[XmlElement(ElementName = "reset_password_validity")]
		public string Reset_password_validity { get; set; }
		[XmlElement(ElementName = "associations")]
		public AssociationsAccount Associations { get; set; }
	}

	[XmlRoot(ElementName = "customers")]
	public class Customers
	{
		[XmlElement(ElementName = "customer")]
		public List<Customer> Customer { get; set; }
	}

	[XmlRoot(ElementName = "prestashop")]
	public class PrestashopAccounts
	{
		[XmlElement(ElementName = "customers")]
		public Customers Customers { get; set; }
		[XmlAttribute(AttributeName = "xlink", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Xlink { get; set; }
	}
}