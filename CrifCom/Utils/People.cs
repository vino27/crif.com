using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace CrifCom.Utils
{
    [Serializable, XmlRoot("person")]
    public class Person
    {
        //[XmlElement("id")]
        // public string id { get; set; }

        [XmlElement("first-name")]
        public string FirstName { get; set; }

        [XmlElement("last-name")]
        public string LastName { get; set; }

        [XmlElement("email-address")]
        public string Email { get; set; }

        [XmlElement("industry")]
        public string Industry { get; set; }

        [XmlArray("positions")]
        [XmlArrayItem("position", Type = typeof(Position))]
        public Position[] Positions { get; set; }

        //[System.Xml.Serialization.XmlElement("positions")]
        //public Positions Positions { get; set; }

        [XmlElement("location")]
        public Location location { get; set; }


    }

    [Serializable, XmlRoot("position")]
    public class Position
    {
        [XmlElement("company")]
        public Company company { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("is-current")]
        public bool isCurrent { get; set; }

        [XmlElement("location")]
        public string location { get; set; }
    }

    [Serializable]
    public class Positions
    {
        [XmlAttribute("total")]
        public int Total { get; set; }

        public Position[] position { get; set; }
    }

    [Serializable]
    public class Company
    {
        [XmlElement("name")]
        public string Name { get; set; }
    }
    [Serializable]
    public class Location
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("country")]
        public Country Country { get; set; }
    }

    [Serializable]
    public class Country
    {
        [XmlElement("code")]
        public string Code { get; set; }
    }

}