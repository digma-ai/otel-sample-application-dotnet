using System;
using Sample.Contracts;

namespace Sample.ReportTracking
{
    public class ReportRequestReceivedEvent : IReportRequestReceivedEvent
    {
        private readonly ReportSagaStateMore _reportSagaStateMore;
        public ReportRequestReceivedEvent(ReportSagaStateMore reportSagaStateMore)
        {
            _reportSagaStateMore = reportSagaStateMore;
        }

        public Guid CorrelationId => _reportSagaStateMore.CorrelationId;

        public string CustomerId => _reportSagaStateMore.CustomerId;
        public string ReportId => _reportSagaStateMore.ReportId;
        public DateTime RequestTime => _reportSagaStateMore.RequestTime;
    }
}
