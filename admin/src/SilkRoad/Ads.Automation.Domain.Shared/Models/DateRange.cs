using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Ads.Automation.Domain.Shared.Models
{
    public class DateRange
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="dateArray"></param>
        public DateRange(ICollection<DateTime> dateArray)
        {
            Start = dateArray.First();
            Stop = dateArray.Last();
        }

        /// <summary>
        /// ctor
        /// </summary>
        public DateRange() { }

        /// <summary>
        /// 开始日期
        /// </summary>
        [JsonPropertyOrder(1)]
        public DateTime Start { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        [JsonPropertyOrder(2)]
        public DateTime Stop { get; set; }

        /// <summary>
        /// 差异天数
        /// </summary>
        [JsonIgnore]
        public int DiffDays => Stop.Subtract(Start).Days;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Start:yyyy-MM-dd HH:mm:ss}-{Stop:yyyy-MM-dd HH:mm:ss}";
        }

        /// <summary>
        /// 返回简写时间范围，相同日期只返回一个日期
        /// </summary>
        /// <returns></returns>
        public string ToShortString()
        {
            return Start.Date == Stop.Date ? $"{Start:yyyy/MM/dd}" : $"{Start:yyyy/MM/dd}~{Stop:yyyy/MM/dd}";
        }

        /// <summary>
        /// 设置开始日期结束日期为 00:00:00 到 23:59:59
        /// </summary>
        public void SetStartEnd()
        {
            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            Stop = new DateTime(Stop.Year, Stop.Month, Stop.Day).AddDays(1).AddSeconds(-1);
        }

        /// <summary>
        /// 获取间隔天数
        /// </summary>
        /// <returns></returns>
        public int GetDays()
        {
            var start = new DateTime(Start.Year, Start.Month, Start.Day);
            var stop = new DateTime(Stop.Year, Stop.Month, Stop.Day);
            return (int)(stop - start).TotalDays + 1;
        }

        /// <summary>
        /// 获取日期列表
        /// </summary>
        /// <returns></returns>
        public List<DateTime> GetDateList()
        {
            var dateList = new List<DateTime>();

            var start = this.Start;
            while (start <= this.Stop)
            {
                dateList.Add(start);

                start = start.AddDays(1);
            }

            return dateList;
        }
    }
}
