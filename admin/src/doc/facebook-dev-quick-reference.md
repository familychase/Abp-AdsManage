---
AIGC:
    Label: "1"
    ContentProducer: 001191440300708461136T1XGW3
    ProduceID: a3427b4dcb331ca28f2cec8946276583_902128a1653711f1aaba5254006c9bbf
    ReservedCode1: Uz6cIQ5JSNuXTnMcQ10ntZZNdMToFMep7FxFfjuHoaKVyEYhTpB2TpMdXOo5LnvW/iYJy0DUQQVkWC0ftCMl98ULU0uiuaVsBUaxP/+2/lx7nVeSzPm71ZAY0seaNbwQc24jmG/3235lHWXPddlhrvOX4bh4DyHP6aWSuYO0uDbKUd7Fx76yvLpBgCM=
    ContentPropagator: 001191440300708461136T1XGW3
    PropagateID: a3427b4dcb331ca28f2cec8946276583_902128a1653711f1aaba5254006c9bbf
    ReservedCode2: Uz6cIQ5JSNuXTnMcQ10ntZZNdMToFMep7FxFfjuHoaKVyEYhTpB2TpMdXOo5LnvW/iYJy0DUQQVkWC0ftCMl98ULU0uiuaVsBUaxP/+2/lx7nVeSzPm71ZAY0seaNbwQc24jmG/3235lHWXPddlhrvOX4bh4DyHP6aWSuYO0uDbKUd7Fx76yvLpBgCM=
---

# Facebook 开发快速参考卡

## 核心信息速查

### API 基础信息
- **当前版本**: v25.0 (2026年2月)
- **版本策略**: 每半年更新，支持2年
- **基础URL**: `https://graph.facebook.com/v25.0/`
- **测试工具**: Graph API Explorer

### 关键限制
- **个人主页发帖**: ❌ 不支持 (2018年后移除)
- **页面发帖**: ✅ 支持 (需要页面权限)
- **最大文件大小**: 1.5GB (可续传)
- **Reels时长**: 3-60秒
- **Reels比例**: 9:16 竖版

## 5分钟快速开始

### 1. 创建应用
```
1. 访问 developers.facebook.com
2. 登录 → My Apps → Create App
3. 选择 Business 类型
4. 获取 App ID 和 App Secret
```

### 2. 获取令牌
```bash
# 短时效令牌 (1-2小时)
通过 Facebook Login 获取

# 长时效令牌 (60天)
GET /oauth/access_token?grant_type=fb_exchange_token&client_id=APP_ID&client_secret=APP_SECRET&fb_exchange_token=SHORT_TOKEN

# 页面令牌 (永不过期)
GET /me/accounts?access_token=LONG_TOKEN
```

### 3. 测试连接
```bash
# 测试用户信息
GET /me?fields=id,name,email&access_token=TOKEN

# 测试页面访问
GET /me/accounts?access_token=TOKEN
```

## 常用 API 端点速查

### 用户相关
| 操作 | 端点 | 权限 |
|------|------|------|
| 获取用户信息 | `GET /me` | `public_profile` |
| 获取用户邮箱 | `GET /me?fields=email` | `email` |
| 获取用户朋友 | `GET /me/friends` | `user_friends` |
| 获取用户页面 | `GET /me/accounts` | `pages_show_list` |

### 页面发帖
| 内容类型 | 端点 | 必需权限 |
|----------|------|----------|
| 文本帖子 | `POST /{page-id}/feed` | `pages_manage_posts` |
| 图片帖子 | `POST /{page-id}/photos` | `pages_manage_posts` |
| 视频帖子 | `POST /{page-id}/videos` | `pages_manage_posts` |
| Reels | `POST /{page-id}/video_reels` | `pages_manage_posts` |
| 链接帖子 | `POST /{page-id}/feed` | `pages_manage_posts` |

### 广告管理
| 操作 | 端点 | 必需权限 |
|------|------|----------|
| 创建广告活动 | `POST /act_{id}/campaigns` | `ads_management` |
| 创建广告组 | `POST /act_{id}/adsets` | `ads_management` |
| 创建广告 | `POST /act_{id}/ads` | `ads_management` |
| 获取洞察 | `GET /{ad-id}/insights` | `ads_read` |

## 权限速查表

### 基础权限 (无需审核)
- `public_profile` - 公开信息
- `email` - 邮箱地址

### 发帖权限 (需要审核)
- `pages_show_list` - 访问页面列表
- `pages_read_engagement` - 读取页面内容
- `pages_manage_posts` - 管理页面帖子

### 广告权限 (需要商业验证)
- `ads_management` - 管理广告
- `ads_read` - 读取广告数据
- `business_management` - 访问商业管理器

## 错误代码速查

| 代码 | 含义 | 解决方案 |
|------|------|----------|
| 4 | 应用频率限制 | 降低请求频率 |
| 17 | 用户频率限制 | 等待后重试 |
| 32 | 页面API限制 | 检查页面权限 |
| 100 | 参数无效 | 检查请求参数 |
| 190 | 令牌无效 | 重新获取令牌 |
| 200 | 权限不足 | 申请相应权限 |
| 613 | Reels频率限制 | 降低发布频率 |

## 请求示例

### 发布文本帖子
```bash
POST /{page-id}/feed
{
  "message": "Hello Facebook!",
  "access_token": "PAGE_TOKEN"
}
```

### 发布带链接帖子
```bash
POST /{page-id}/feed
{
  "message": "Check this out",
  "link": "https://example.com",
  "access_token": "PAGE_TOKEN"
}
```

### 发布图片
```bash
POST /{page-id}/photos
{
  "url": "https://example.com/image.jpg",
  "message": "Beautiful picture",
  "access_token": "PAGE_TOKEN"
}
```

### 定时发布
```bash
POST /{page-id}/feed
{
  "message": "Scheduled post",
  "published": false,
  "scheduled_publish_time": 1710324000,
  "access_token": "PAGE_TOKEN"
}
```

## 频率限制速查

### 应用级限制
- **公式**: 200 × 应用用户数 / 小时
- **监控**: `X-App-Usage` 响应头

### 页面级限制
- **公式**: 4800 × 互动用户数 / 天
- **监控**: `X-Business-Use-Case-Usage` 响应头

### 处理策略
1. 监控使用率头信息
2. 实现指数退避 (1s, 2s, 4s...)
3. 均匀分布请求
4. 缓存频繁访问的数据

## 安全最佳实践

### 令牌安全
- ✅ 服务器端存储 App Secret
- ✅ 使用环境变量
- ✅ 定期轮换令牌
- ❌ 前端暴露敏感信息
- ❌ 提交到版本控制

### 权限管理
- 最小权限原则
- 定期审查权限
- 及时移除未使用权限

### Webhook 安全
- 验证签名
- 验证来源
- 处理重放攻击

## 开发工具链

### 测试工具
1. **Graph API Explorer** - 在线测试API
2. **Access Token Debugger** - 调试令牌
3. **Webhook Tester** - 测试Webhook

### 监控工具
1. **App Dashboard** - 应用监控
2. **Business Manager** - 商业监控
3. **第三方监控** - 自定义监控

### 部署工具
1. **环境变量管理**
2. **密钥管理服务**
3. **CI/CD 集成**

## 紧急问题处理

### 令牌失效
1. 检查令牌调试器
2. 重新获取令牌
3. 更新应用配置

### API 错误
1. 检查错误代码
2. 查看官方文档
3. 搜索开发者社区

### 频率限制
1. 降低请求频率
2. 实现退避策略
3. 联系Meta支持

## 版本迁移检查清单

### 迁移前
- [ ] 查看版本弃用通知
- [ ] 测试新版本API
- [ ] 更新依赖库

### 迁移中
- [ ] 更新API版本号
- [ ] 处理行为变更
- [ ] 更新错误处理

### 迁移后
- [ ] 全面测试
- [ ] 监控错误率
- [ ] 更新文档

---

**黄金法则**:
1. 先测试再上线
2. 服务器端处理敏感操作
3. 始终指定API版本
4. 监控频率限制
5. 准备错误处理

**一句话记住**: 创建应用 → 获取权限 → 使用API → 注意版本和限制
*（内容由AI生成，仅供参考）*
*（内容由AI生成，仅供参考）*
