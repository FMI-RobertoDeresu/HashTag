using System;
using System.Collections.Generic;
using HashTag.Domain.AutoMapping;
using HashTag.Domain.Dtos;

namespace HashTag.Presentation.Models.Admin
{
    public class ClustersChartGroupModel : IMapFrom<ClustersChartGroupDto>
    {
        public long Id { get; set; }

        public string Label { get; set; }

        public string Color { get; set; }

        public Tuple<decimal, decimal> CoreLocation { get; set; }

        public IEnumerable<Tuple<decimal, decimal>> ItemsLocations { get; set; }
    }
}