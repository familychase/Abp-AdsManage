/*
 * @author: huangk 2022-12-7 18:41:43
 */

using static Ads.Automation.Infrastructure.SDK.Models.Meta.Domain.MetaDomain;

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Input
{

    public partial class MetaInput
    {
        /// <summary>
        /// 广告组更新参数
        /// </summary>
        public class AdSetUpdateParameter
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string? name { get; set; }

            /// <summary>
            /// 竞价金额
            /// </summary>
            public long? bid_amount { get; set; }

            /// <summary>
            /// 每日预算
            /// </summary>
            public long? daily_budget { get; set; }

            /// <summary>
            /// 终身预算
            /// </summary>
            public long? lifetime_budget { get; set; }

            /// <summary>
            /// 广告组结束时间
            /// </summary>
            public DateTime? end_time { get; set; }

            /// <summary>
            /// 目标
            /// </summary>
            public MetaDomain.Targeting? targeting { get; set; }

            /// <summary>
            /// 状态
            /// enum{ACTIVE, PAUSED, DELETED, ARCHIVED}
            /// </summary>
            public string? status { get; set; }


            /// <summary>
            /// 每天最小消耗目标
            /// </summary>
            public long? daily_min_spend_target { get; set; }

            /// <summary>
            /// 每日消费上限
            /// </summary>
            public long? daily_spend_cap { get; set; }

            /// <summary>
            /// 终身最小消耗目标
            /// </summary>
            public long? lifetime_min_spend_target { get; set; }

            /// <summary>
            /// 终身消耗上限
            /// </summary>
            public long? lifetime_spend_cap { get; set; }

            /// <summary>
            /// 该广告集中所有广告的受益人
            /// </summary>
            public string? dsa_beneficiary { get; set; }

            /// <summary>
            /// 该广告集中所有广告的付款人
            /// </summary>
            public string? dsa_payor { get; set; }

            /// <summary>
            /// 优化目标为价值时传入
            /// </summary>
            public BidContraint? bid_constraints { get; set; }

            /// <summary>
            /// 修改固定预算
            /// </summary>
            /// <param name="metaLifetimeBudget">终身预算</param>
            /// <param name="value">当前要修改为的预算</param>
            /// <param name="metaDailyBudget">日预算</param>
            /// <param name="isDailyBudget">是否为日预算</param>
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
                //未设置预算，无法修改
                if (long.TryParse(metaDailyBudget, out var v) && v > 0)
                {
                    //总预算大于100 按100处理  小于100计算实际值
                    this.daily_budget = CalculateIncreaseDecreaseBudget(metaDailyBudget, value);
                }
                //未设置预算，无法修改
                else if (!string.IsNullOrEmpty(metaLifetimeBudget))
                {
                    //总预算大于100 按100处理  小于100计算实际值
                    this.lifetime_budget = CalculateIncreaseDecreaseBudget(metaLifetimeBudget, value);

                    isDailyBudget = false;
                }
                else
                {
                    throw new NullReferenceException("预算不允许修改！");
                }
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
                    this.daily_budget = CalculatePercentBudget(metaDailyBudget,value);
                }
                //未设置终身预算，无法修改
                else if (!string.IsNullOrEmpty(metaLifetimeBudget))
                {
                    this.lifetime_budget = CalculatePercentBudget(metaDailyBudget, value);

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
            private long CalculateIncreaseDecreaseBudget(string strBudget, decimal value)
            {
                var budget = Convert.ToDecimal(strBudget) / 100M;

                //设置增减值预算
                budget = Math.Round(budget + value, 0, MidpointRounding.AwayFromZero) * 100;

                return Convert.ToInt64(budget < 100 ? 100 : budget);
            }

            /// <summary>
            /// 计算百分比预算
            /// </summary>
            /// <param name="strBudget"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            private long CalculatePercentBudget(string strBudget, decimal value)
            {
                var budget = Convert.ToDecimal(strBudget) / 100M;

                //设置百分比预算 
                budget = Math.Round(budget * (1 + value), 0,MidpointRounding.AwayFromZero) * 100;

                return Convert.ToInt64(budget < 100 ? 100 : budget);
            }

            /// <summary>
            /// 修改状态
            /// </summary>
            public void SetStatus(string adSetStatus)
            {
                this.status = adSetStatus;
            }
        }

    }

}
