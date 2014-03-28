using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtiWrapperServices
{
    /// <summary>
    /// AtiWrapperServices QualifiedObjects Class 
    /// </summary>
    public class QualifiedObjects
    {
        /// <summary>
        /// Properties
        /// </summary>
        public string QualifiedName { get; set; } 
        public List<object> QualifiedObjectList{ get; set;}

        /// <summary>
        /// AtiWrapperServices QualifiedObjects default constructor
        /// </summary>
        public QualifiedObjects()
        {
            QualifiedName = String.Empty;
            QualifiedObjectList = new List<object>();
        }

        /// <summary>
        /// AtiWrapperServices QualifiedObjects constructor with name
        /// </summary>
        /// <param name="name">AtiWrapperServices QualifiedObjects Name</param>
        public QualifiedObjects(string name)
        {
            QualifiedName = name;
            QualifiedObjectList = new List<object>();
        }
    }
}