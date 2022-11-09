using System;
using Sample.Contracts;

namespace Sample.ReportTracking
{
    public class ReportCreatedEvent : IReportCreatedEvent
    {
        private readonly ReportSagaStateMore _reportSagaStateMore;
        public ReportCreatedEvent(ReportSagaStateMore reportSagaStateMore)
        {
            _reportSagaStateMore = reportSagaStateMore;
        }

        public Guid CorrelationId => _reportSagaStateMore.CorrelationId;

        public string CustomerId => _reportSagaStateMore.CustomerId;
        public string ReportId => _reportSagaStateMore.ReportId;

        public string BlobUri => _reportSagaStateMore.BlobUri;

        public DateTime CreationTime => _reportSagaStateMore.CreationTime;
    }
}
