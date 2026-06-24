-- =============================================
-- 像素表 (ads_pixel)
-- 像素-账户关联表 (ads_account_pixel)
-- 生成日期: 2026-06-18
-- 数据库: SQL Server
-- 用途: 存储 Meta 广告像素信息及账户与像素的关联关系
-- =============================================

-- ============================================
-- ads_pixel: 像素信息表（纯像素数据，不绑定账户）
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ads_pixel]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[ads_pixel] (
        [Id]           BIGINT          NOT NULL,   -- 雪花 ID（IdGenerator 生成）
        [PixelNo]      NVARCHAR(128)   NOT NULL,   -- Meta 像素编号（全局唯一）
        [PixelName]    NVARCHAR(256)   NOT NULL,   -- 像素名称
        [Code]         NVARCHAR(MAX)   NULL,       -- 像素追踪代码（JS）
        [LastSyncTime] DATETIME2       NOT NULL,   -- 最后同步时间
        [CreationTime] DATETIME2       NOT NULL,   -- 创建时间

        CONSTRAINT [PK_ads_pixel] PRIMARY KEY CLUSTERED ([Id])
    );

    -- 按 PixelNo 唯一约束
    CREATE UNIQUE NONCLUSTERED INDEX [IX_ads_pixel_PixelNo]
        ON [dbo].[ads_pixel] ([PixelNo]);
END
GO

-- ============================================
-- ads_account_pixel: 账户-像素关联表（多对多）
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ads_account_pixel]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[ads_account_pixel] (
        [Id]        BIGINT         NOT NULL IDENTITY(1,1),  -- 自增主键
        [AccountNo] NVARCHAR(128)  NOT NULL,                -- Meta 广告账户编号
        [PixelId]   BIGINT         NOT NULL,                -- 关联 ads_pixel.Id
        [PixelNo]   NVARCHAR(128)  NULL,                    -- 像素编号（冗余，方便查询）

        CONSTRAINT [PK_ads_account_pixel] PRIMARY KEY CLUSTERED ([Id])
    );

    -- 按 AccountNo 查询该账户下的所有像素
    CREATE NONCLUSTERED INDEX [IX_ads_account_pixel_AccountNo]
        ON [dbo].[ads_account_pixel] ([AccountNo]);

    -- 同一账户下同一像素不重复
    CREATE UNIQUE NONCLUSTERED INDEX [IX_ads_account_pixel_AccountNo_PixelId]
        ON [dbo].[ads_account_pixel] ([AccountNo], [PixelId]);
END
GO
