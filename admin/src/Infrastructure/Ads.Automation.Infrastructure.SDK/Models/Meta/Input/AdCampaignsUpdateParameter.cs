/*
 * @author: huangk 2022-12-7 18:41:43
 */

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Input
{
    public partial class MetaInput
    {
        /// <summary>
        /// 更新广告系列参数
        /// </summary>
        public class AdCampaignsUpdateParameter
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string? name { get; set; }

            /// <summary>
            /// 每日预算。
            /// </summary>
            public long? daily_budget { get; set; }

            /// <summary>
            /// 终身预算
            /// </summary>
            public long? lifetime_budget { get; set; }

            /// <summary>
            /// 状态
            /// enum{ACTIVE, PAUSED, DELETED, ARCHIVED}
            /// </summary>
            public string? status { get; set; }


            /// <summary>
            /// 修改固定预算
            /// </summary>
            /// <param name="metaLifetimeBudget"></param>
            /// <param name="value">当前要修改为的预算</param>
            /// <param name="metaDailyBudget"></param>
            /// <param name="isDailyBudget"></param>
            public void SetFixedBudget(string metaDailyBudget, string metaLifetimeBudget, decimal value, ref bool isDailyBudget)
            {
                if (long.TryParse(metaDailyBudget, out var v) && v > 0)
                {
                    //未设置预算，无法修改
                    this.daily_budget = Convert.ToInt64(value * 100);
                }
                else if (!string.IsNullOrEmpty(metaLifetimeBudget))
                {
                    //未设置终身预算  无法修改
                    this.lifetime_budget = Convert.ToInt64(value * 100);

                    isDailyBudget = false;
                }
                else
                {
                    throw new NullReferenceException("预算不允许修改！");
                }
            }


            /// <summary>
            /// 设置增减值预算
            /// </summary>
            /// <param name="metaDailyBudget">当前广告组媒体每日预算</param>
            /// <param name="metaLifetimeBudget">当前广告组媒体终身预算</param>
            /// <param name="value">当前要修改为的预算</param>
            /// <param name="isDailyBudget">是否是每日预算</param>
            public void SetIncreaseDecreaseBudget(string metaDailyBudget, string metaLifetimeBudget, decimal value, ref bool isDailyBudget)
            {
                value = Math.Round(value, 0);

                if (long.TryParse(metaDailyBudget, out var v) && v > 0)
                {
                    //未设置预算，无法修改
                    this.daily_budget = CalculateIncreaseDecreaseBudget(metaDailyBudget, value);
                }
                else if (!string.IsNullOrEmpty(metaLifetimeBudget))
                {
                    //未设置终身预算  无法修改
                    this.lifetime_budget = CalculateIncreaseDecreaseBudget(metaLifetimeBudget, value);

                    isDailyBudget = false;
                }
                else
                {
                    throw new NullReferenceException("预算不允许修改！");
                }
            }

            /// <summary>
            /// 计算增减值预算
            /// </summary>
            /// <param name="strBudget"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            private long CalculateIncreaseDecreaseBudget(string strBudget,decimal value)
            {
                var budget = Convert.ToDecimal(strBudget) / 100M;

                //设置增减值预算
                budget = Math.Round(budget + value, 0) * 100;

                return Convert.ToInt64(budget < 100 ? 100 : budget);
            }


            /// <summary>
            /// 设置增减值百分比预算
            /// </summary>
            /// <param name="metaDailyBudget">当前广告组媒体每日预算</param>
            /// <param name="metaLifetimeBudget">当前广告组媒体终身预算</param>
            /// <param name="value">当前要修改为的预算</param>
            /// <param name="isDailyBudget">是否是每日预算</param>
            public void SetIncreaseDecreasePercentBudget(string metaDailyBudget, string metaLifetimeBudget, decimal value, ref bool isDailyBudget)
            {
                //未设置每日预算，无法修改
                if (long.TryParse(metaDailyBudget, out var v) && v > 0)
                {
                    //未设置预算，无法修改
                    this.daily_budget = CalculatePercentBudget(metaDailyBudget, value);
                }
                else if (!string.IsNullOrEmpty(metaLifetimeBudget))
                {
                    //未设置预算，无法修改
                    this.lifetime_budget = CalculatePercentBudget(metaLifetimeBudget, value);

                    isDailyBudget = false;
                }
                else
                {
                    throw new NullReferenceException("预算不允许修改！");
                }
            }


            /// <summary>
            /// 计算百分比预算
            /// </summary>
            /// <param name="strBudget"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            private long CalculatePercentBudget(string strBudget,decimal value)
            { 
                var budget = Convert.ToDecimal(strBudget) / 100M;

                //设置百分比预算 
                budget = Math.Round(budget * (1 + value), 0) * 100;

                return Convert.ToInt64(budget < 100 ? 100 : budget);
            }

            /// <summary>
            /// 修改广告系列状态
            /// </summary>
            /// <param name="campaignStatus">修改状态</param>
            public void SetStatus(string campaignStatus)
            {
                this.status = campaignStatus;
            }
        }

    }
}
