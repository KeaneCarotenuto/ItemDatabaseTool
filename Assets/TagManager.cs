using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

// tag manager class for items
public class TagManager
{
    public class Tag
    {
        readonly public string m_id = "";

        private string m_name = "";
        public string name 
        {
            get { 
                return m_name; 
            }

            /// <summary>
            /// Sets the name of the tag. ONLY letters, numbers, and underscores are allowed.
            /// </summary>
            set
            {
                m_name = value;
                ValidateName();
            }
        }

        public void ValidateName()
        {
            //correct name
            //to lower
            m_name = m_name.ToLower();
            //replace non letters and numbers with ""
            m_name = Regex.Replace(m_name, @"[^a-zA-Z0-9_]", "");
            //replace space with _
            m_name = m_name.Replace(" ", "_");
        }

        private string m_payload = "";
        public string payload
        {
            get { 
                return m_payload; 
            }
            set { 
                m_payload = value; 
            }
        }

        // constructor
        public Tag(string _name)
        {
            // needs unique id
            m_id = this.GetType().Name + "_" + Guid.NewGuid().ToString();
            this.name = _name;
            m_payload = "";
        }
    }

    // PAYLOAD TYPES
    

}