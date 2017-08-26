using System;
using System.Collections.Generic;

namespace HashTag.Domain.Dtos
{
    public class ClustersChartGroupDto
    {
        public long Id { get; set; }

        public string Label { get; set; }

        public string Color { get; set; }

        public Tuple<decimal, decimal> CoreLocation { get; set; }

        public IEnumerable<Tuple<decimal, decimal>> ItemsLocations { get; set; }
    }
}