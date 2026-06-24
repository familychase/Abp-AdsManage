---
AIGC:
    Label: "1"
    ContentProducer: 001191440300708461136T1XGW3
    ProduceID: a3427b4dcb331ca28f2cec8946276583_8fb1912b653711f1a4b9525400d9a7a1
    ReservedCode1: ZsBAho/BHwFqcxMgEzDEnVcKs2pYwYTXK+8hqqpap58/4vJU4MJPUT2YWul9Lf73XtDDlnmPnsq8XJY2CidRpd7tpzHMIkLc+s5eS7P2gxeu9oKRu4B9KmAeIppePk1wTRVNiwsXIVKjk/M/mBccxgHjCSGSVXllg5AVUJjCuCv9cDjM0WIyGfpFj2s=
    ContentPropagator: 001191440300708461136T1XGW3
    PropagateID: a3427b4dcb331ca28f2cec8946276583_8fb1912b653711f1a4b9525400d9a7a1
    ReservedCode2: ZsBAho/BHwFqcxMgEzDEnVcKs2pYwYTXK+8hqqpap58/4vJU4MJPUT2YWul9Lf73XtDDlnmPnsq8XJY2CidRpd7tpzHMIkLc+s5eS7P2gxeu9oKRu4B9KmAeIppePk1wTRVNiwsXIVKjk/M/mBccxgHjCSGSVXllg5AVUJjCuCv9cDjM0WIyGfpFj2s=
---

# Facebook/Meta 开发者技能指南

## 技能概述
**技能名称**: `facebook-dev-expert`
**用途**: 为开发者提供 Facebook/Meta 平台 API 的快速参考、问题解答和最佳实践指导

## 核心 API 概览

### 1. Facebook Graph API（核心数据接口）
- **用途**: 读取/写入 Facebook 社交图谱数据
- **当前版本**: v25.0（2026年2月发布）
- **版本策略**: 每半年发布新版本，支持约2年
- **基础 URL**: `https://graph.facebook.com/v25.0/`

### 2. Facebook Login API（用户认证）
- **用途**: 使用 Facebook 账号登录第三方应用
- **认证协议**: OAuth 2.0
- **核心流程**: 获取用户访问令牌 → 调用 Graph API

### 3. Facebook Marketing API（广告管理）
- **用途**: 程序化管理广告活动、受众、创意
- **核心权限**: `ads_management`, `ads_read`, `business_management`
- **系统用户**: 通过 Business Manager 创建系统用户获取令牌

### 4. WhatsApp Business API（商业消息）
- **用途**: 发送/接收 WhatsApp 商业消息
- **类型**: Cloud API（推荐）、On-premises API（已弃用）
- **消息模板**: 需预先审核

### 5. Instagram API（内容管理）
- **用途**: 管理 Instagram 商业账号内容
- **功能**: 发布内容、获取帖子、管理评论、分析洞察

## 快速入门指南

### 第一步：创建开发者应用
1. 访问 [Meta for Developers](https://developers.facebook.com/)
2. 登录 Facebook 账号
3. 创建应用 → 选择类型（Business 最简单）
4. 获取 App ID 和 App Secret

### 第二步：获取访问令牌
```bash
# 短时效用户令牌（1-2小时）
通过 Facebook Login 对话框获取

# 长时效用户令牌（约60天）
GET /oauth/access_token?grant_type=fb_exchange_token&client_id={app-id}&client_secret={app-secret}&fb_exchange_token={short-lived-token}

# 页面令牌（永不过期）
GET /{user-id}/accounts?access_token={long-lived-token}
```

### 第三步：测试 API
```bash
# 使用 Graph API Explorer
https://developers.facebook.com/tools/explorer/

# 测试用户信息
GET https://graph.facebook.com/v25.0/me?fields=id,name,email&access_token=YOUR_TOKEN
```

## 权限系统详解

### 权限层级
1. **基础权限**（无需审核）:
   - `public_profile`: 公开信息
   - `email`: 邮箱地址

2. **高级权限**（需要审核）:
   - `pages_manage_posts`: 管理页面帖子
   - `pages_read_engagement`: 读取页面内容
   - `pages_show_list`: 访问页面列表

3. **商业权限**（需商业验证）:
   - `ads_management`: 管理广告
   - `business_management`: 访问 Business Manager

### 应用审核要求
- **详细说明**: 解释为什么需要该权限
- **屏幕录像**: 展示完整用户流程
- **隐私政策**: 可访问的隐私政策
- **测试账号**: 审核员可登录的测试账号

## 发布内容到 Facebook 页面

### 文本帖子
```bash
POST /{page-id}/feed
{
  "message": "帖子内容",
  "access_token": "PAGE_TOKEN"
}
```

### 带链接的帖子
```bash
POST /{page-id}/feed
{
  "message": "帖子内容",
  "link": "https://example.com",
  "access_token": "PAGE_TOKEN"
}
```

### 定时发布
```bash
POST /{page-id}/feed
{
  "message": "定时发布内容",
  "published": false,
  "scheduled_publish_time": 1710324000,  # UNIX时间戳
  "access_token": "PAGE_TOKEN"
}
```

### 发布图片
```bash
POST /{page-id}/photos
{
  "url": "https://example.com/photo.jpg",
  "message": "图片说明",
  "access_token": "PAGE_TOKEN"
}
```

### 发布视频（三步流程）
1. **开始上传会话**:
   ```bash
   POST /{app-id}/uploads
   {
     "file_name": "video.mp4",
     "file_type": "video/mp4",
     "access_token": "PAGE_TOKEN"
   }
   ```

2. **上传文件**:
   ```bash
   POST /upload:{UPLOAD_SESSION_ID}
   授权头: Authorization: OAuth PAGE_TOKEN
   请求体: 视频二进制数据
   ```

3. **发布视频**:
   ```bash
   POST /{page-id}/videos
   {
     "description": "视频描述",
     "fbuploader_video_file_chunk": "UPLOADED_FILE_HANDLE",
     "access_token": "PAGE_TOKEN"
   }
   ```

### 发布 Reels（短视频）
1. **初始化上传**:
   ```bash
   POST /{page-id}/video_reels
   {
     "access_token": "PAGE_TOKEN"
   }
   ```

2. **上传视频**:
   ```bash
   POST https://rupload.facebook.com/video-upload/{video-id}
   授权头: Authorization: OAuth PAGE_TOKEN
   请求体: file_url=https://example.com/reel.mp4
   ```

3. **发布**:
   ```bash
   POST /{page-id}/video_reels
   {
     "upload_phase": "finish",
     "video_id": "VIDEO_ID",
     "video_state": "PUBLISHED",
     "description": "Reel说明",
     "access_token": "PAGE_TOKEN"
   }
   ```

## 广告 API 快速使用

### 创建广告活动
```bash
POST /act_{ad-account-id}/campaigns
{
  "name": "夏季促销",
  "objective": "OUTCOME_TRAFFIC",
  "status": "PAUSED",
  "special_ad_categories": [],
  "access_token": "AD_ACCOUNT_TOKEN"
}
```

### 创建广告组
```bash
POST /act_{ad-account-id}/adsets
{
  "name": "移动端用户",
  "campaign_id": "{campaign-id}",
  "daily_budget": "1000",
  "billing_event": "IMPRESSIONS",
  "optimization_goal": "REACH",
  "targeting": {
    "geo_locations": {"countries": ["US"]},
    "age_min": 18,
    "age_max": 65
  },
  "access_token": "AD_ACCOUNT_TOKEN"
}
```

### 创建广告
```bash
POST /act_{ad-account-id}/ads
{
  "name": "产品广告",
  "adset_id": "{adset-id}",
  "creative": {
    "object_story_spec": {
      "page_id": "{page-id}",
      "link_data": {
        "link": "https://example.com",
        "message": "点击查看详情"
      }
    }
  },
  "status": "PAUSED",
  "access_token": "AD_ACCOUNT_TOKEN"
}
```

## 错误处理与限制

### 常见错误码
| 错误码 | 含义 | 解决方案 |
|--------|------|----------|
| 4 | 应用级频率限制 | 降低请求频率 |
| 17 | 用户级频率限制 | 等待后重试 |
| 32 | 页面API限制 | 检查页面权限 |
| 100 | 参数错误 | 检查请求参数 |
| 190 | 令牌无效 | 重新获取令牌 |
| 613 | Reels频率限制 | 降低Reels发布频率 |

### 频率限制
- **应用级**: 200 × 应用用户数 / 小时
- **页面级**: 4800 × 互动用户数 / 天
- **响应头**: `X-App-Usage`, `X-Business-Use-Case-Usage`

### 令牌失效场景
1. 用户修改密码
2. 用户取消应用授权
3. 用户失去页面管理员权限
4. 重置应用密钥

## 最佳实践

### 开发建议
1. **始终指定API版本**: 避免默认版本变更导致问题
2. **使用环境变量**: 保护 App Secret 和访问令牌
3. **实现错误重试**: 使用指数退避策略
4. **监控使用情况**: 关注响应头中的使用率指标

### 安全建议
1. **服务器端处理**: 所有敏感操作在服务器端进行
2. **最小权限原则**: 只请求必要的权限
3. **定期更新令牌**: 处理令牌失效场景
4. **验证Webhook签名**: 确保Webhook请求来自Meta

### 性能优化
1. **批量请求**: 使用 `batch` 端点减少请求数
2. **字段选择**: 只请求需要的字段
3. **缓存策略**: 缓存不常变化的数据
4. **异步处理**: 长时间操作使用异步方式

## 工具与资源

### 官方工具
1. **Graph API Explorer**: 在线测试API
2. **Access Token Debugger**: 调试访问令牌
3. **App Dashboard**: 管理应用设置
4. **Business Manager**: 管理商业资产

### 第三方服务
1. **Postproxy**: 简化多平台发布
2. **Data365**: 社交媒体数据API
3. **Adstellar**: 广告API集成工具
4. **Hyperleap**: WhatsApp API服务

### 学习资源
1. **官方文档**: developers.facebook.com/docs
2. **开发者社区**: Meta for Developers 论坛
3. **GitHub示例**: Meta官方示例代码库
4. **Stack Overflow**: `facebook-graph-api` 标签

## 常见问题解答

### Q: 能发布到个人主页吗？
A: **不能**。2018年后只能发布到Facebook页面（Page）。

### Q: 应用审核需要多久？
A: 通常1-3个工作日，复杂情况可能更长。

### Q: 如何获取永不过期的页面令牌？
A: 使用长时效用户令牌获取页面令牌。

### Q: 视频上传大小限制？
A: 单次请求1GB/20分钟，可续传1.5GB/45分钟。

### Q: Reels有什么特殊要求？
A: 3-60秒，9:16竖版，1080p以上分辨率。

### Q: 如何处理频率限制？
A: 实现指数退避，监控使用率头信息。

## 版本迁移指南

### 检查弃用项
1. 查看当前版本弃用通知
2. 测试新版本API
3. 更新代码中的版本号
4. 处理行为变更

### 迁移时间线
- 新版本发布后，旧版本支持约2年
- 提前3个月开始迁移
- 在生产环境切换前充分测试

---

**一句话总结**: Facebook开发 = 创建应用 → 获取权限 → 使用Graph API → 注意版本和限制。先测试再上线，服务器端处理敏感操作。
*（内容由AI生成，仅供参考）*
*（内容由AI生成，仅供参考）*
