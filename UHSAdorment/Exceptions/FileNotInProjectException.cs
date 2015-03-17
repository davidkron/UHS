using System;
using System.Runtime.Serialization;

namespace Exceptions
{
	[Serializable]
	internal class FileNotInProjectException : Exception
	{
		public FileNotInProjectException()
		{
		}

		public FileNotInProjectException(string message) : base(message)
		{
		}

		public FileNotInProjectException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected FileNotInProjectException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}