#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the ClassGenerator.ttinclude code generation file.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Common;
using System.Collections.Generic;
using Telerik.OpenAccess;
using Telerik.OpenAccess.Metadata;
using Telerik.OpenAccess.Data.Common;
using Telerik.OpenAccess.Metadata.Fluent;
using Telerik.OpenAccess.Metadata.Fluent.Advanced;
using System.ComponentModel.DataAnnotations;
using AtiWrapperServicesORM.OpenAccess;

namespace AtiWrapperServicesORM.OpenAccess	
{
	public partial class ServiceName
	{
		private int _id;
		[Required()]
		[Key()]
		public virtual int Id
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
			}
		}
		
		private string _name;
		[Required()]
		public virtual string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}
		
		private int _serviceURL_Id;
		[Required()]
		public virtual int ServiceURL_Id
		{
			get
			{
				return this._serviceURL_Id;
			}
			set
			{
				this._serviceURL_Id = value;
			}
		}
		
		private int _serviceType;
		[Required()]
		public virtual int ServiceType
		{
			get
			{
				return this._serviceType;
			}
			set
			{
				this._serviceType = value;
			}
		}
		
		private ServiceURL _serviceURL;
		public virtual ServiceURL ServiceURL
		{
			get
			{
				return this._serviceURL;
			}
			set
			{
				this._serviceURL = value;
			}
		}
		
		private IList<ServiceMethod> _serviceMethods = new List<ServiceMethod>();
		public virtual IList<ServiceMethod> ServiceMethods
		{
			get
			{
				return this._serviceMethods;
			}
		}
		
	}
}
#pragma warning restore 1591
