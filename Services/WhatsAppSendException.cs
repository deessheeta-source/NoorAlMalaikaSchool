using System;

namespace NoorAlMalaikaSchool.Services
{
    /// <summary>
    /// Describes a Cloud API send failure, including whether retrying is safe
    /// and whether the provider may already have accepted the message.
    /// </summary>
    public sealed class WhatsAppSendException : Exception
    {
        public bool IsRetryable { get; private set; }
        public int? HttpStatus { get; private set; }
        public TimeSpan? RetryAfter { get; private set; }
        public bool IsOutcomeUnknown { get; private set; }

        public WhatsAppSendException(string message, bool isRetryable, int? httpStatus,
            TimeSpan? retryAfter, bool isOutcomeUnknown, Exception inner)
            : base(message, inner)
        {
            IsRetryable = isRetryable;
            HttpStatus = httpStatus;
            RetryAfter = retryAfter;
            IsOutcomeUnknown = isOutcomeUnknown;
        }
    }
}
