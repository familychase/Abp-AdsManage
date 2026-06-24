-- =============================================
-- 复制明细表 (ads_duplicate_detail)
-- 生成日期: 2026-06-11
-- 最后更新: 2026-06-17 (CampaignNo→AdObjectNo、新增AdObjectLevel)
-- 数据库: SQL Server
-- 用途: 记录每次广告对象创建的成败明细，用于后续补充/删除
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ads_duplicate_detail]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[ads_duplicate_detail] (
        [Id]           BIGINT          NOT NULL,
        [LogId]        BIGINT          NOT NULL,   -- 关联 ads_duplicate_logging.Id
        [Index]        INT             NOT NULL,   -- 第几次迭代 (1..CopyNumber)
        [AdObjectLevel] TINYINT        NOT NULL,   -- 广告层级: 1=CAMPAIGN,2=AD_SET
        [AdObjectNo]   NVARCHAR(128)   NOT NULL,   -- 新创建的广告对象编号(CampaignId/AdSetId)
        [State]        TINYINT         NOT NULL,   -- 0=PENDING,1=SUCCESS,2=FAILED,3=IN_PROGRESS
        [ErrorMessage] NVARCHAR(MAX)   NULL,       -- 失败原因
        [Content]      NVARCHAR(MAX)   NULL,       -- 创建内容详情
        [CreateTime]   DATETIME2       NOT NULL,   -- 创建时间

        CONSTRAINT [PK_ads_duplicate_detail] PRIMARY KEY CLUSTERED ([Id])
    );

    -- 按 LogId 查询某一复制任务的全部明细
    CREATE NONCLUSTERED INDEX [IX_ads_duplicate_detail_LogId]
        ON [dbo].[ads_duplicate_detail] ([LogId]);

    -- 按 AdObjectNo 查询，用于后续补充/删除
    CREATE NONCLUSTERED INDEX [IX_ads_duplicate_detail_AdObjectNo]
        ON [dbo].[ads_duplicate_detail] ([AdObjectNo]);
END
GO
