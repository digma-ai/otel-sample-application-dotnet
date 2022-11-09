using System;
using Sample.Contracts;

namespace Sample.ReportTracking
{
    public class ReportFailedEvent : IReportFailedEvent
    {
        private readonly ReportSagaStateMore _reportSagaStateMore;
        public ReportFailedEvent(ReportSagaStateMore reportSagaStateMore)
        {
            _reportSagaStateMore = reportSagaStateMore;
        }

        public Guid CorrelationId => _reportSagaStateMore.CorrelationId;

        public string CustomerId => _reportSagaStateMore.CustomerId;
        public string ReportId => _reportSagaStateMore.ReportId;
        public string FaultMessage => _reportSagaStateMore.FaultMessage;
        public DateTime FaultTime => _reportSagaStateMore.FaultTime;
    }
}
