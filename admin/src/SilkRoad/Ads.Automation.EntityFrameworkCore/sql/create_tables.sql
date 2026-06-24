-- =============================================
-- Ads.Automation 新建表 SQL
-- 数据库：SQL Server
-- 日期：2026-05-29
-- =============================================

-- 1. 广告复制日志表
IF  EXISTS (SELECT * FROM sysobjects WHERE name = 'ads_duplicate_logging' AND xtype = 'U')
BEGIN
drop table  ads_duplicate_logging
END
GO

CREATE TABLE ads_duplicate_logging
(
    Id                  BIGINT          NOT NULL PRIMARY KEY,
    DuplicateSource     TINYINT         NOT NULL,         -- 复制来源: 0=ADS_MANAGEMENT, 1=STRATEGY
    ResourceId          BIGINT          NOT NULL DEFAULT 0,-- 来源ID
    IsInternal          BIT             NOT NULL DEFAULT 1,-- 是否账户内复制
    AdObjectLevel       TINYINT         NOT NULL,         -- 广告对象层级: 0=CAMPAIGN, 1=AD_SET
    AdObjectNo          NVARCHAR(100)   NOT NULL,         -- 广告对象编号
    AccountNo           NVARCHAR(50)    NOT NULL,         -- 账户编号
    DuplicateAccountNo  NVARCHAR(50)    NOT NULL DEFAULT '',-- 目标账户号
    PageNo              NVARCHAR(50)    NOT NULL DEFAULT '',-- 公共主页编号
    State               TINYINT         NOT NULL DEFAULT 0,-- 状态: 0=PENDING, 1=SUCCESS, 2=FAILED, 3=IN_PROGRESS
    DuplicateContent    NVARCHAR(MAX)   NULL,             -- 复制结果内容
    ScheduleTime        DATETIME2       NOT NULL,         -- 计划执行时间
    EndTime             DATETIME2       NULL,             -- 结束时间
    ExtendedData        NVARCHAR(MAX)   NULL DEFAULT '',  -- 扩展数据(JSON)
    CopyNumber          BIGINT          NOT NULL DEFAULT 1,-- 复制数量
    ErrorMessage        NVARCHAR(MAX)   NULL DEFAULT '',  -- 错误信息(媒体返回错误或代码异常)
    CreateTime          DATETIME2       NOT NULL,         -- 创建时间
    CreateBy            BIGINT          NOT NULL DEFAULT 0 -- 创建人
);
CREATE INDEX IX_ads_duplicate_logging_State_ScheduleTime
    ON ads_duplicate_logging (State, ScheduleTime)
    WHERE State = 0;  -- PENDING 状态索引
CREATE INDEX IX_ads_duplicate_logging_AccountNo
    ON ads_duplicate_logging (AccountNo);

-- 2. 公共主页同步表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'ads_page' AND xtype = 'U')
BEGIN
drop table  ads_page
END
GO

CREATE TABLE ads_page
(
    Id              BIGINT          NOT NULL PRIMARY KEY,
    PageNo          NVARCHAR(50)    NOT NULL,         -- Meta 主页编号
    PageName        NVARCHAR(200)   NOT NULL DEFAULT '',-- 主页名称
    Category        NVARCHAR(200)   NULL,             -- 主页分类
    AccessToken     NVARCHAR(500)   NULL,             -- 主页访问令牌
    AccountNo       NVARCHAR(50)    NOT NULL DEFAULT '',-- 关联广告账户号
    Platform        TINYINT         NOT NULL DEFAULT 2,-- 媒体平台: 2=META
    LastSyncTime    DATETIME2       NOT NULL,         -- 最后同步时间
    CreationTime    DATETIME2       NOT NULL          -- 创建时间
);
CREATE UNIQUE INDEX IX_ads_page_PageNo
    ON ads_page (PageNo);
CREATE INDEX IX_ads_page_AccountNo
    ON ads_page (AccountNo);

-- 4. Meta API 调用统计表
IF  EXISTS (SELECT * FROM sysobjects WHERE name = 'ads_meta_api_usage' AND xtype = 'U')
BEGIN
drop table  ads_meta_api_usage
END
GO

CREATE TABLE ads_meta_api_usage
(
    Id                  BIGINT          NOT NULL PRIMARY KEY,
    AccountNo           NVARCHAR(50)    NOT NULL,
    TimeSlot            DATETIME2       NOT NULL,
    TotalCalls          BIGINT          NOT NULL DEFAULT 0,
    TotalPoints         BIGINT          NOT NULL DEFAULT 0,
    MethodStats         NVARCHAR(MAX)   NOT NULL DEFAULT '{}',
    RateLimitHits       BIGINT          NOT NULL DEFAULT 0,
    LastCallTime        DATETIME2       NULL,
    LastRateLimitTime   DATETIME2       NULL,
    CreationTime        DATETIME2       NOT NULL,
    LastModificationTime DATETIME2      NULL
);
CREATE UNIQUE INDEX IX_ads_meta_api_usage_AccountNo_TimeSlot
    ON ads_meta_api_usage (AccountNo, TimeSlot);

-- 5. 广告发布模板表
IF  EXISTS (SELECT * FROM sysobjects WHERE name = 'ads_publish_templates' AND xtype = 'U')
BEGIN
drop table  ads_publish_templates
END
GO

CREATE TABLE ads_publish_templates
(
    Id                              BIGINT          NOT NULL PRIMARY KEY,
    Version                         INT             NOT NULL DEFAULT 0,     -- 版本号（Meta=1，其他=0）
    Name                            NVARCHAR(100)   NOT NULL,               -- 模板名称
    Platform                        TINYINT         NOT NULL,               -- 媒体平台: 1=GOOGLE, 2=META, 3=TIKTOK, 4=SNAPCHAT
    PublishingAdType                TINYINT         NOT NULL,               -- 发布广告类型: 1=APP, 2=PIXEL, 3=PRODUCT_CATALOG, 4=PERFORMANCE_MAX, 5=DISPLAY, 6=MULTI_CHANNEL
    ResourceId                      BIGINT          NOT NULL DEFAULT 0,     -- 发布对象Id（application/pixel/catalog）
    ResourceContent                 NVARCHAR(500)   NOT NULL DEFAULT '',    -- 来源网址
    PublishCount                    INT             NOT NULL DEFAULT 0,     -- 累计发布次数
    PublishAdCount                  INT             NOT NULL DEFAULT 0,     -- 累计发布广告数
    BatchPublishingType             TINYINT         NOT NULL DEFAULT 0,     -- 批量发布类型: 0=NONE, 1=AVG, 2=ALL
    BatchMaxPublishCount            INT             NOT NULL DEFAULT 0,     -- 每组最大发布广告数
    BatchPublishAverage             BIT             NOT NULL DEFAULT 0,     -- 是否尽量平均发布
    TemplateContent                 NVARCHAR(MAX)   NOT NULL,               -- 模板内容（JSON）
    LastPublishTime                 DATETIME2       NOT NULL,               -- 末次发布时间
    IsDeleted                       BIT             NOT NULL DEFAULT 0,     -- 是否删除
    DeleterId                       BIGINT          NULL,                   -- 删除人
    DeletionTime                    DATETIME2       NULL,                   -- 删除时间
    CreatorId                       BIGINT          NOT NULL DEFAULT 0,     -- 创建人
    CreationTime                    DATETIME2       NOT NULL,               -- 创建时间
    LastModifierId                  BIGINT          NULL,                   -- 最后修改人
    LastModificationTime            DATETIME2       NULL                    -- 最后修改时间
);

CREATE INDEX IX_ads_publish_templates_Platform
    ON ads_publish_templates (Platform);

CREATE INDEX IX_ads_publish_templates_IsDeleted
    ON ads_publish_templates (IsDeleted)
    WHERE IsDeleted = 0;

-- 6、受众数据模型
IF  EXISTS (SELECT * FROM sysobjects WHERE name = 'ads_publish_audience' AND xtype = 'U')
BEGIN
    DROP TABLE ads_publish_audience
END
GO

CREATE TABLE ads_publish_audience
(
    Id                              BIGINT          NOT NULL PRIMARY KEY,     -- 主键
    Platform                        TINYINT         NOT NULL,                 -- 媒体平台
    Type                            TINYINT         NOT NULL,                 -- 发布受众类型
    ParentId                        BIGINT          NOT NULL DEFAULT 0,       -- 父级id
    Name                            NVARCHAR(200)   NOT NULL,                 -- 名称
    NameEN                          NVARCHAR(200)   NOT NULL,                 -- 名称 - English
    Code                            NVARCHAR(100)   NOT NULL,                 -- 编码
    Remark                          NVARCHAR(500)   NOT NULL DEFAULT '',      -- 备注信息
);