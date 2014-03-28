using System;
using System.Text;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ServiceModel.Description;

namespace DynamicServiceProxyNamespace
{
    /// <summary>
    /// DynamicServiceProxyException Class for exceptions that happen when creating and running Dynamic Service proxies
    /// </summary>
    public class DynamicServiceProxyException : ApplicationException
    {
        private IEnumerable<MetadataConversionError> importErrors = null;
        private IEnumerable<MetadataConversionError> codegenErrors = null;
        private IEnumerable<CompilerError> compilerErrors = null;


        /// <summary>
        /// Dynamic service proxy exception constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        public DynamicServiceProxyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Dynamic service proxy exception constructor with inner exception
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public DynamicServiceProxyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Metadata import errors getter and setter
        /// </summary>
        public IEnumerable<MetadataConversionError> MetadataImportErrors
        {
            get
            {
                return this.importErrors;
            }

            internal set
            {
                this.importErrors = value;
            }
        }
        /// <summary>
        /// Code Generation Errors getter and setter
        /// </summary>
        public IEnumerable<MetadataConversionError> CodeGenerationErrors
        {
            get
            {
                return this.codegenErrors;
            }

            internal set
            {
                this.codegenErrors = value;
            }
        }

        /// <summary>
        /// Compilation Errors getter and setter
        /// </summary>
        public IEnumerable<CompilerError> CompilationErrors
        {
            get
            {
                return this.compilerErrors;
            }

            internal set
            {
                this.compilerErrors = value;
            }
        }

        /// <summary>
        /// Returns a string with all of the errors
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(base.ToString());

            if (MetadataImportErrors != null)
            {
                builder.AppendLine("Metadata Import Errors:");
                builder.AppendLine(DynamicServiceProxyFactory.ToString(
                            MetadataImportErrors));
            }

            if (CodeGenerationErrors != null)
            {
                builder.AppendLine("Code Generation Errors:");
                builder.AppendLine(DynamicServiceProxyFactory.ToString(
                            CodeGenerationErrors));
            }

            if (CompilationErrors != null)
            {
                builder.AppendLine("Compilation Errors:");
                builder.AppendLine(DynamicServiceProxyFactory.ToString(
                            CompilationErrors));
            }

            return builder.ToString();
        }
    }
}