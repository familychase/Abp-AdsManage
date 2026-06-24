using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Application.Contracts.Entity.Menus
{
    public class SysMenusMapDto
    {
        public string Path { get; set; } = "/";
        public string Component { get; set; }
        public string Name { get; set; }
        public string Redirect { get; set; }
        public RouteMetaCustom Meta { get; set; } = new RouteMetaCustom();
        public List<SysMenusMapDto> Children { get; set; }
    }

    public class RouteMetaCustom
    {
        public bool? Hidden { get; set; }
        public bool? AlwaysShow { get; set; }
        public string? Title { get; set; }
        public string? Icon { get; set; }
    }
}
