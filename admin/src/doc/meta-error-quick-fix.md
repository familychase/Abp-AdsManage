---
AIGC:
    Label: "1"
    ContentProducer: 001191440300708461136T1XGW3
    ProduceID: a3427b4dcb331ca28f2cec8946276583_90bf0d99653711f1aaba5254006c9bbf
    ReservedCode1: 2bQEwCXjjqVWtF7tBZYDvY+8m6jXKDO4/+FcDvl/szkll94QebmG7Y2eA16DCSmi0ZZ4egLEKRAFVUaKl7Swl8pj9JVzaMeA0Rh5P2ks+vyX/btl42bDEtgLb7n+gPT5/eHnjXrL7HP6JhS7DPj8yPY/wmsV8/MGL8OT6DlLWtHl9aPNt1JJm5vWh6A=
    ContentPropagator: 001191440300708461136T1XGW3
    PropagateID: a3427b4dcb331ca28f2cec8946276583_90bf0d99653711f1aaba5254006c9bbf
    ReservedCode2: 2bQEwCXjjqVWtF7tBZYDvY+8m6jXKDO4/+FcDvl/szkll94QebmG7Y2eA16DCSmi0ZZ4egLEKRAFVUaKl7Swl8pj9JVzaMeA0Rh5P2ks+vyX/btl42bDEtgLb7n+gPT5/eHnjXrL7HP6JhS7DPj8yPY/wmsV8/MGL8OT6DlLWtHl9aPNt1JJm5vWh6A=
---

# Meta 错误快速修复卡

## 紧急情况速查表

### 遇到错误时，按此顺序排查

```
1. 检查错误代码
2. 验证访问令牌
3. 确认API版本
4. 检查应用状态
5. 查看频率限制
```

## 错误代码速查表

| 错误码 | 类型 | 紧急程度 | 第一步操作 |
|--------|------|----------|------------|
| **#4** | 应用频率限制 | 🔴 高 | 降低请求频率，检查X-App-Usage头 |
| **#17** | 用户频率限制 | 🟡 中 | 等待1小时后重试，实现退避策略 |
| **#32** | 页面API限制 | 🟡 中 | 检查页面权限，验证页面令牌 |
| **#100** | 参数错误 | 🟢 低 | 检查请求参数，使用API Explorer测试 |
| **#190** | 令牌无效 | 🔴 高 | 使用调试工具检查令牌，重新获取 |
| **#200** | 权限不足 | 🔴 高 | 验证权限，检查应用审核状态 |
| **#613** | Reels限制 | 🟡 中 | 降低Reels发布频率，等待恢复 |
| **80001** | 页面账户限制 | 🟡 中 | 检查页面绑定状态，重新授权 |

## 5分钟紧急修复指南

### 情况1: 令牌突然失效
**症状**: 之前正常的API调用返回#190错误

**紧急处理**:
```bash
# 1. 立即调试令牌
访问: https://developers.facebook.com/tools/debug/accesstoken/

# 2. 检查令牌状态
- 是否过期? (Expires)
- 权限是否完整? (Scopes)
- 应用是否有效? (App ID)

# 3. 重新获取令牌
# 短时效 → 长时效 → 页面令牌
```

### 情况2: 频率限制被触发
**症状**: 返回#4, #17, #32错误

**紧急处理**:
```python
# 立即实现指数退避
import time
import random

def make_request_with_backoff():
    for attempt in range(5):
        try:
            # 你的API调用
            return api_call()
        except RateLimitError:
            if attempt == 4:
                raise
            
            # 退避时间: 2^attempt + 随机抖动
            wait = (2 ** attempt) + random.uniform(0, 1)
            time.sleep(wait)
            continue
```

### 情况3: 权限错误(#200)
**症状**: 获取数据失败，提示权限不足

**紧急处理**:
```bash
# 1. 检查必需权限
必需权限列表:
- pages_show_list (访问页面列表)
- pages_read_engagement (读取页面内容) 
- pages_manage_posts (管理页面帖子)
- ads_management (管理广告)

# 2. 验证权限
使用调试工具检查令牌包含的权限

# 3. 临时解决方案
- 切换到开发模式
- 使用管理员账号令牌
- 申请紧急审核(如需要)
```

### 情况4: API端点不存在
**症状**: "Unknown path components"错误

**紧急处理**:
```bash
# 1. 验证端点是否存在
使用Graph API Explorer测试:
https://developers.facebook.com/tools/explorer/

# 2. 常见错误端点修正
❌ /me/adaccountgroups  → ✅ /me/adaccounts
❌ /me/feed (POST)      → ✅ 使用Share Dialog
❌ 未指定版本          → ✅ /v18.0/端点

# 3. 检查文档时效性
访问最新官方文档:
https://developers.facebook.com/docs/
```

## 调试命令速查

### PowerShell/CMD 快速测试
```powershell
# 测试API连通性
curl "https://graph.facebook.com/v18.0/me?fields=id,name&access_token=YOUR_TOKEN"

# 调试令牌
curl "https://graph.facebook.com/debug_token?input_token=YOUR_TOKEN&access_token=APP_TOKEN"
```

### 浏览器快速测试
```javascript
// 在浏览器控制台测试
fetch('https://graph.facebook.com/v18.0/me?fields=id,name&access_token=YOUR_TOKEN')
  .then(r => r.json())
  .then(console.log)
  .catch(console.error)
```

## 应用状态检查清单

### 开发模式 vs 公开模式
| 模式 | 特点 | 适合场景 |
|------|------|----------|
| **开发模式** | 仅管理员可用，无需审核 | 开发测试阶段 |
| **公开模式** | 所有用户可用，需要审核 | 生产环境 |

### 检查应用健康状态
```bash
# 1. 登录开发者后台
# 2. 检查应用状态
- ✅ 应用是否有效?
- ✅ 密钥是否安全?
- ✅ 回调URL是否正确?
- ✅ 测试用户是否配置?

# 3. 查看审核状态
- 基础权限: 自动通过
- 高级权限: 需要审核
- 商业权限: 需要商业验证
```

## 频率限制监控

### 响应头监控
```bash
# 关键响应头
X-App-Usage: {"call_count": 28, "total_cputime": 5, "total_time": 10}
# call_count > 80% 时预警
# total_time > 90% 时立即降频

X-Business-Use-Case-Usage: {"估计恢复时间": 30}
# 恢复时间 > 0 时暂停请求
```

### 预防性措施
```python
class RateLimitMonitor:
    def __init__(self):
        self.usage_history = []
    
    def check_headers(self, response_headers):
        # 解析使用率
        app_usage = self.parse_usage_header(response_headers.get('X-App-Usage'))
        biz_usage = self.parse_usage_header(response_headers.get('X-Business-Use-Case-Usage'))
        
        # 预警逻辑
        if app_usage.get('call_count', 0) > 80:
            self.throttle_requests()
        if biz_usage.get('estimated_time_to_regain_access', 0) > 0:
            self.pause_requests(biz_usage['estimated_time_to_regain_access'])
```

## 临时解决方案

### 当审核被卡住时
```bash
# 临时继续开发
1. 切换回开发模式
2. 使用管理员账号测试
3. 准备审核材料:
   - 详细功能说明文档
   - 屏幕录像(3-5分钟)
   - 隐私政策链接
   - 测试账号信息
```

### 当令牌频繁失效时
```python
# 实现自动令牌刷新
class TokenManager:
    def __init__(self):
        self.token = None
        self.expires_at = None
    
    def get_valid_token(self):
        if self.token is None or self.is_expired():
            self.refresh_token()
        return self.token
    
    def refresh_token(self):
        # 实现令牌刷新逻辑
        # 短时效 → 长时效 → 页面令牌
        pass
    
    def is_expired(self):
        return time.time() > self.expires_at - 300  # 提前5分钟刷新
```

## 联系支持指南

### 何时联系Meta支持
- ✅ 应用被错误标记为非活跃
- ✅ 审核被无故拒绝
- ✅ 商业验证卡住
- ✅ API行为与文档不符

### 联系前准备
```bash
# 必需信息
1. 应用ID: _________
2. 开发者账号ID: _________
3. 错误详情: _________
4. 复现步骤: _________
5. 截图/日志: _________

# 可选信息
- 请求ID (从错误响应中获取)
- 时间戳
- 相关交易ID
```

### 支持渠道
1. **开发者论坛**: 社区互助
2. **支持中心**: 提交工单
3. **紧急联系**: 商业客户专线
4. **文档反馈**: 报告文档问题

## 恢复验证清单

### 修复后验证步骤
```bash
# 逐步验证
[ ] 1. 基础连通性测试: GET /me
[ ] 2. 权限验证测试: 检查必需权限
[ ] 3. 业务功能测试: 核心API调用
[ ] 4. 错误处理测试: 模拟错误场景
[ ] 5. 性能测试: 检查频率限制
[ ] 6. 回归测试: 原有功能验证
```

### 监控指标
```bash
# 关键监控指标
- 错误率: < 1%
- 响应时间: < 2秒
- 令牌刷新成功率: > 99%
- 审核通过率: (如适用)
```

---

## 紧急联系信息

### 重要链接
- **令牌调试**: https://developers.facebook.com/tools/debug/accesstoken/
- **API测试**: https://developers.facebook.com/tools/explorer/
- **文档首页**: https://developers.facebook.com/docs/
- **支持中心**: https://developers.facebook.com/support/

### 紧急备用方案
1. **降级方案**: 使用简化功能版本
2. **缓存方案**: 使用缓存数据临时替代
3. **手动方案**: 准备人工操作流程
4. **通知用户**: 透明沟通服务状态

**记住**: 保持冷静，按步骤排查，大多数错误都有已知解决方案。
*（内容由AI生成，仅供参考）*
*（内容由AI生成，仅供参考）*
