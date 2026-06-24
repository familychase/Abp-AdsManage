-- =============================================
-- 系统字典表组
-- 生成日期: 2026-06-22
-- 数据库: SQL Server
-- 用途: 系统字典类/字典项
-- =============================================

-- =============================================
-- 1. 系统字典类表 (sys_dict_sort)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sys_dict_sort]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[sys_dict_sort] (
        [Id]                    BIGINT          NOT NULL,
        [Platform]              TINYINT         NOT NULL    CONSTRAINT [DF_sys_dict_sort_Platform] DEFAULT (0),   -- 媒体平台: 0=NONE,1=GOOGLE,2=META,3=TIKTOK
        [DictSortType]          TINYINT         NOT NULL    CONSTRAINT [DF_sys_dict_sort_DictSortType] DEFAULT (0), -- 字典类型: 0=NONE,1=SYSTEM,2=MEDIA
        [DictSortCode]          NVARCHAR(64)    NOT NULL,   -- 字典类编码(唯一)
        [DictSortName]          NVARCHAR(128)   NOT NULL,   -- 字典类名称
        [Remarks]               NVARCHAR(500)   NULL,       -- 备注信息
        [CreatorId]             BIGINT          NOT NULL    CONSTRAINT [DF_sys_dict_sort_CreatorId] DEFAULT (0),
        [CreationTime]          DATETIME2       NOT NULL,
        [LastModifierId]        BIGINT          NULL,
        [LastModificationTime]  DATETIME2       NULL,

        CONSTRAINT [PK_sys_dict_sort] PRIMARY KEY CLUSTERED ([Id])
    );

    -- 字典类编码唯一索引
    CREATE UNIQUE NONCLUSTERED INDEX [IX_sys_dict_sort_DictSortCode]
        ON [dbo].[sys_dict_sort] ([DictSortCode]);
END
GO


-- =============================================
-- 2. 系统字典项表 (sys_dict_item)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sys_dict_item]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[sys_dict_item] (
        [Id]                    BIGINT          NOT NULL,
        [DictSortId]            BIGINT          NOT NULL,   -- 所属字典类Id
        [ParentId]              BIGINT          NOT NULL    CONSTRAINT [DF_sys_dict_item_ParentId] DEFAULT (0), -- 父级字典项Id(0=顶级)
        [DictItemCode]          NVARCHAR(64)    NOT NULL,   -- 字典项编码
        [DictItemName]          NVARCHAR(128)   NOT NULL,   -- 字典项名称(中文)
        [DictItemNameEN]        NVARCHAR(128)   NULL,       -- 字典项名称(英文)
        [DictItemValue]         NVARCHAR(256)   NULL,       -- 字典项值
        [Remarks]               NVARCHAR(500)   NULL,       -- 备注信息
        [Ordinal]               INT             NOT NULL    CONSTRAINT [DF_sys_dict_item_Ordinal] DEFAULT (0), -- 序号
        [ItemType]              TINYINT         NOT NULL    CONSTRAINT [DF_sys_dict_item_ItemType] DEFAULT (0), -- 值类型: 0=NONE,1=TIME_SECOND,2=PERCENT,3=CURRENCY,4=NUMBER
        [IsProduction]          BIT             NOT NULL    CONSTRAINT [DF_sys_dict_item_IsProduction] DEFAULT (1), -- 环境标识: 1=正式,0=测试
        [CreatorId]             BIGINT          NOT NULL    CONSTRAINT [DF_sys_dict_item_CreatorId] DEFAULT (0),
        [CreationTime]          DATETIME2       NOT NULL,
        [LastModifierId]        BIGINT          NULL,
        [LastModificationTime]  DATETIME2       NULL,

        CONSTRAINT [PK_sys_dict_item] PRIMARY KEY CLUSTERED ([Id])
    );

    -- 同一字典类下编码唯一
    CREATE UNIQUE NONCLUSTERED INDEX [IX_sys_dict_item_DictSortId_DictItemCode]
        ON [dbo].[sys_dict_item] ([DictSortId], [DictItemCode]);

  
END
GO

PRINT '✔ 系统字典表组创建完成 (sys_dict_sort, sys_dict_item)';
