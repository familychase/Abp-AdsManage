<script setup lang="tsx">
import { ContentWrap } from '@/components/ContentWrap'
import { useI18n } from '@/hooks/web/useI18n'
import { Table } from '@/components/Table'
import { ElTag, ElInput, ElMessage, ElDialog, ElTabs, ElTabPane } from 'element-plus'
import { ref, unref, computed, watch } from 'vue'
import { useTable } from '@/hooks/web/useTable'
import { CrudSchema, useCrudSchemas } from '@/hooks/web/useCrudSchemas'
import { BaseButton } from '@/components/Button'

import { getSysLogErrorListApi } from '@/api/system/logError'
import type { SysLogErrorDto } from '@/api/system/logError/types'

const { t } = useI18n()

// ══════════════════════════════════════════
//  Tab 状态
// ══════════════════════════════════════════

/** 当前激活的 tab */
const activeTab = ref<'error_log' | 'info_log' | 'warning_log' | 'other_log'>('error_log')

/** 各级别对应 tab 的提示色 */
const TAB_TAG_TYPE: Record<string, 'danger' | 'info' | 'warning' | 'primary'> = {
  error_log: 'danger',
  info_log: 'info',
  warning_log: 'warning',
  other_log: 'primary'
}

// ══════════════════════════════════════════
//  查询条件（error_log 专用，其他 tab 暂未实现）
// ══════════════════════════════════════════

const searchLevel = ref('')
const searchLogger = ref('')
const searchRequestPath = ref('')
const searchKeyword = ref('')

// ══════════════════════════════════════════
//  表格 & 数据（使用 useTable 钩子）
// ══════════════════════════════════════════

const { tableRegister, tableState, tableMethods } = useTable({
  fetchDataApi: async () => {
    if (activeTab.value !== 'error_log') {
      return { list: [], total: 0 }
    }
    const res = await getSysLogErrorListApi({
      page: unref(currentPage),
      pageSize: unref(pageSize),
      level: searchLevel.value || undefined,
      logger: searchLogger.value || undefined,
      requestPath: searchRequestPath.value || undefined,
      keyword: searchKeyword.value || undefined
    })
    const data = res.data as any
    return {
      list: data?.items ?? [],
      total: data?.totalCount ?? 0
    }
  }
})

const { loading, dataList, total, currentPage, pageSize } = tableState
const { getList } = tableMethods

const handleSearch = () => {
  currentPage.value = 1
  getList()
}

const handleReset = () => {
  searchLevel.value = ''
  searchLogger.value = ''
  searchRequestPath.value = ''
  searchKeyword.value = ''
  currentPage.value = 1
  getList()
}

/** 切换 tab 时清空数据并刷新（其他 tab 暂不请求接口） */
const handleTabChange = (tab: string | number | undefined) => {
  if (typeof tab === 'string') {
    activeTab.value = tab as typeof activeTab.value
  }
  currentPage.value = 1
  if (activeTab.value === 'error_log') {
    getList()
  } else {
    // 其他 tab 暂不请求，清空当前数据
    dataList.value = []
    total.value = 0
  }
}

// ══════════════════════════════════════════
//  列定义
// ══════════════════════════════════════════

/** 级别 → Tag 类型 */
const LEVEL_TAG_TYPE: Record<string, 'danger' | 'warning' | 'info' | 'success'> = {
  ERROR: 'danger',
  FATAL: 'danger',
  WARN: 'warning',
  WARNING: 'warning',
  INFO: 'info',
  DEBUG: 'success',
  TRACE: 'success'
}

const crudSchemas = computed<CrudSchema[]>(() => [
  {
    field: 'creationTime',
    label: t('messageLog.creationTime'),
    search: { hidden: true },
    table: {
      width: '180px',
      slots: {
        default: (data: any) => {
          const val = data.row.creationTime ?? data.row.CreationTime ?? '-'
          return <span>{val}</span>
        }
      }
    }
  },
  {
    field: 'level',
    label: t('messageLog.level'),
    search: { hidden: true },
    table: {
      width: '90px',
      slots: {
        default: (data: any) => {
          const row: SysLogErrorDto = data.row
          const lv = (row.level || 'ERROR').toUpperCase()
          return (
            <ElTag type={LEVEL_TAG_TYPE[lv] || 'danger'} size="small">
              {lv}
            </ElTag>
          )
        }
      }
    }
  },
  {
    field: 'logger',
    label: t('messageLog.logger'),
    search: { hidden: true },
    table: {
      minWidth: '180px',
      slots: {
        default: (data: any) => {
          const val = data.row.logger ?? data.row.Logger ?? '-'
          return <span>{val}</span>
        }
      }
    }
  },
  {
    field: 'requestPath',
    label: t('messageLog.requestPath'),
    search: { hidden: true },
    table: {
      minWidth: '220px',
      slots: {
        default: (data: any) => {
          const method = data.row.method ?? data.row.Method ?? ''
          const path = data.row.requestPath ?? data.row.RequestPath ?? '-'
          return (
            <span style="font-family: 'JetBrains Mono', Consolas, monospace; font-size: 12px;">
              {method ? `[${method}] ` : ''}
              {path}
            </span>
          )
        }
      }
    }
  },
  {
    field: 'statusCode',
    label: t('messageLog.statusCode'),
    search: { hidden: true },
    table: {
      width: '90px',
      align: 'center',
      slots: {
        default: (data: any) => {
          const code = data.row.statusCode ?? data.row.StatusCode
          if (code === undefined || code === null) return <span>-</span>
          const type = code >= 500 ? 'danger' : code >= 400 ? 'warning' : 'info'
          return (
            <ElTag type={type} size="small" effect="plain">
              {code}
            </ElTag>
          )
        }
      }
    }
  },
  {
    field: 'userName',
    label: t('messageLog.user'),
    search: { hidden: true },
    table: {
      width: '120px',
      slots: {
        default: (data: any) => {
          const val = data.row.userName ?? data.row.UserName ?? '-'
          return <span>{val}</span>
        }
      }
    }
  },
  {
    field: 'clientIp',
    label: t('messageLog.clientIp'),
    search: { hidden: true },
    table: {
      width: '140px',
      slots: {
        default: (data: any) => {
          const val = data.row.clientIp ?? data.row.ClientIp ?? '-'
          return <span>{val}</span>
        }
      }
    }
  },
  {
    field: 'message',
    label: t('messageLog.message'),
    search: { hidden: true },
    table: {
      minWidth: '280px',
      slots: {
        default: (data: any) => {
          const val = data.row.message ?? data.row.Message ?? '-'
          return (
            <el-tooltip content={val} placement="top" show-after={300}>
              <span
                style="
                  display: inline-block;
                  max-width: 380px;
                  overflow: hidden;
                  text-overflow: ellipsis;
                  white-space: nowrap;
                  vertical-align: middle;
                "
              >
                {val}
              </span>
            </el-tooltip>
          )
        }
      }
    }
  },
  {
    field: 'action',
    label: t('tableDemo.action'),
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true },
    table: {
      fixed: 'right',
      width: '110px',
      slots: {
        default: (data: any) => (
          <BaseButton type="primary" size="small" onClick={() => openDetailDialog(data.row)}>
            {t('messageLog.viewDetail')}
          </BaseButton>
        )
      }
    }
  }
])

// 初始化和监听 locale / schema 变化时重新生成 allSchemas
const { allSchemas } = useCrudSchemas(crudSchemas.value)
watch(crudSchemas, (val) => {
  const { allSchemas: fresh } = useCrudSchemas(val)
  Object.assign(allSchemas, fresh)
})

// ══════════════════════════════════════════
//  详情弹窗
// ══════════════════════════════════════════

const detailDialogVisible = ref(false)
const detailRow = ref<SysLogErrorDto | null>(null)
const stackExpanded = ref(false)

const openDetailDialog = (row: SysLogErrorDto) => {
  detailRow.value = { ...row }
  stackExpanded.value = false
  detailDialogVisible.value = true
}

/** 解析字段值：兼容 camelCase / PascalCase */
const pickField = (...keys: string[]): string => {
  if (!detailRow.value) return '-'
  for (const k of keys) {
    const camel = k.charAt(0).toLowerCase() + k.slice(1)
    const v = (detailRow.value as any)[camel] ?? (detailRow.value as any)[k] ?? null
    if (v !== null && v !== undefined && v !== '') return String(v)
  }
  return '-'
}

const toggleStack = () => {
  stackExpanded.value = !stackExpanded.value
}

/** 安全的剪贴板写入，回退到 document.execCommand */
const writeClipboard = async (text: string): Promise<boolean> => {
  try {
    if (navigator.clipboard && window.isSecureContext) {
      await navigator.clipboard.writeText(text)
      return true
    }
  } catch {
    // clipboard API 不可用，尝试 fallback
  }
  // fallback: textarea + execCommand
  const ta = document.createElement('textarea')
  ta.value = text
  ta.style.position = 'fixed'
  ta.style.left = '-9999px'
  ta.style.top = '-9999px'
  document.body.appendChild(ta)
  ta.focus()
  ta.select()
  try {
    document.execCommand('copy')
    return true
  } catch {
    return false
  } finally {
    document.body.removeChild(ta)
  }
}

const copyMessage = async () => {
  const text = rawMessage.value
  if (!text || text === '-') return
  const ok = await writeClipboard(text)
  if (ok) {
    ElMessage.success(t('messageLog.copySuccess'))
  } else {
    ElMessage.error(t('messageLog.copyFailed'))
  }
}

const copyFullLog = async () => {
  if (!detailRow.value) return
  const parts: string[] = []
  parts.push(`Time: ${pickField('creationTime', 'CreationTime')}`)
  parts.push(`Level: ${pickField('level', 'Level')}`)
  parts.push(`Logger: ${pickField('logger', 'Logger')}`)
  parts.push(`Path: [${pickField('method', 'Method')}] ${pickField('requestPath', 'RequestPath')}`)
  parts.push(`User: ${pickField('userName', 'UserName')}`)
  parts.push(`IP: ${pickField('clientIp', 'ClientIp')}`)
  const msg = rawMessage.value
  if (msg && msg !== '-') parts.push(`\nMessage:\n${msg}`)
  const exType = exceptionType.value
  if (exType && exType !== '-') parts.push(`\nException Type: ${exType}`)
  const exMsg = exceptionMessage.value
  if (exMsg && exMsg !== '-') parts.push(`Exception Message: ${exMsg}`)
  const stack = exceptionStack.value
  if (stack && stack !== '-') parts.push(`\nStack:\n${stack}`)
  const ok = await writeClipboard(parts.join('\n'))
  if (ok) {
    ElMessage.success(t('messageLog.copySuccess'))
  } else {
    ElMessage.error(t('messageLog.copyFailed'))
  }
}

const exceptionStack = computed(() => pickField('exception', 'Exception'))
const rawMessage = computed(() => pickField('message', 'Message'))
const exceptionType = computed(() => pickField('exceptionType', 'ExceptionType'))
const exceptionMessage = computed(() => pickField('exceptionMessage', 'ExceptionMessage'))

/** 简短消息：优先用 exceptionMessage，否则取 message 第一行 */
const shortMessage = computed(() => {
  const exMsg = exceptionMessage.value
  if (exMsg && exMsg !== '-') return exMsg
  const raw = rawMessage.value
  if (!raw || raw === '-') return ''
  return raw
    .split('\n')[0]
    .replace(/^System\.\w+Exception:\s*/, '')
    .trim()
})

const hasStack = computed(() => exceptionStack.value && exceptionStack.value !== '-')
const hasShortMessage = computed(() => shortMessage.value !== '')
const hasExceptionType = computed(() => exceptionType.value && exceptionType.value !== '-')
const stackLineCount = computed(() => {
  if (!hasStack.value) return 0
  return exceptionStack.value.split('\n').filter((l) => l.trim()).length
})
</script>

<template>
  <ContentWrap>
    <ElTabs v-model="activeTab" type="border-card" @tab-change="handleTabChange">
      <!-- 错误日志：已实现 -->
      <ElTabPane name="error_log">
        <template #label>
          <span>
            <ElTag :type="TAB_TAG_TYPE.error_log" size="small" effect="dark" class="tab-tag">
              Error
            </ElTag>
            {{ t('messageLog.tabError') }}
          </span>
        </template>

        <!-- 搜索栏 -->
        <div class="search-bar">
          <div class="search-bar__left">
            <ElInput
              v-model="searchLevel"
              :placeholder="t('messageLog.levelPlaceholder')"
              clearable
              style="width: 140px"
              @keyup.enter="handleSearch"
              @clear="handleSearch"
            />
            <ElInput
              v-model="searchLogger"
              :placeholder="t('messageLog.loggerPlaceholder')"
              clearable
              style="width: 200px"
              @keyup.enter="handleSearch"
              @clear="handleSearch"
            />
            <ElInput
              v-model="searchRequestPath"
              :placeholder="t('messageLog.requestPathPlaceholder')"
              clearable
              style="width: 240px"
              @keyup.enter="handleSearch"
              @clear="handleSearch"
            />
            <ElInput
              v-model="searchKeyword"
              :placeholder="t('messageLog.keywordPlaceholder')"
              clearable
              style="width: 220px"
              @keyup.enter="handleSearch"
              @clear="handleSearch"
            />
            <BaseButton type="primary" @click="handleSearch">
              {{ t('common.query') }}
            </BaseButton>
            <BaseButton @click="handleReset">
              {{ t('common.reset') }}
            </BaseButton>
          </div>
        </div>

        <!-- 表格 -->
        <Table
          v-model:pageSize="pageSize"
          v-model:currentPage="currentPage"
          :columns="allSchemas.tableColumns"
          :data="dataList"
          :loading="loading"
          :pagination="{ total }"
          @register="tableRegister"
        />
      </ElTabPane>

      <!-- 信息日志：预留 -->
      <ElTabPane name="info_log">
        <template #label>
          <span>
            <ElTag :type="TAB_TAG_TYPE.info_log" size="small" effect="dark" class="tab-tag">
              Info
            </ElTag>
            {{ t('messageLog.tabInfo') }}
          </span>
        </template>
        <div class="placeholder">
          <el-empty :description="t('messageLog.placeholder')" />
        </div>
      </ElTabPane>

      <!-- 警告日志：预留 -->
      <ElTabPane name="warning_log">
        <template #label>
          <span>
            <ElTag :type="TAB_TAG_TYPE.warning_log" size="small" effect="dark" class="tab-tag">
              Warn
            </ElTag>
            {{ t('messageLog.tabWarning') }}
          </span>
        </template>
        <div class="placeholder">
          <el-empty :description="t('messageLog.placeholder')" />
        </div>
      </ElTabPane>

      <!-- 其他日志：预留 -->
      <ElTabPane name="other_log">
        <template #label>
          <span>
            <ElTag :type="TAB_TAG_TYPE.other_log" size="small" effect="dark" class="tab-tag">
              Other
            </ElTag>
            {{ t('messageLog.tabOther') }}
          </span>
        </template>
        <div class="placeholder">
          <el-empty :description="t('messageLog.placeholder')" />
        </div>
      </ElTabPane>
    </ElTabs>

    <!-- 详情弹窗 -->
    <ElDialog
      v-model="detailDialogVisible"
      width="680px"
      top="8vh"
      destroy-on-close
      :show-close="false"
      :close-on-click-modal="false"
    >
      <div v-if="detailRow" class="err-dlg">
        <!-- 自定义头部 -->
        <div class="err-dlg__header">
          <div class="err-dlg__title-row">
            <span class="err-dlg__title">{{ t('messageLog.detailTitle') }}</span>
            <ElTag type="danger" size="small" effect="dark">ERROR</ElTag>
          </div>
          <button class="err-dlg__close" @click="detailDialogVisible = false">
            <svg width="16" height="16" viewBox="0 0 20 20" fill="currentColor">
              <path
                fill-rule="evenodd"
                d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z"
                clip-rule="evenodd"
              />
            </svg>
          </button>
        </div>

        <!-- 元信息行 -->
        <div class="err-dlg__meta">
          <div class="err-dlg__meta-item">
            <span class="err-dlg__meta-label">{{ t('messageLog.creationTime') }}</span>
            <span class="err-dlg__meta-value">{{ pickField('creationTime', 'CreationTime') }}</span>
          </div>
          <div class="err-dlg__meta-divider"></div>
          <div class="err-dlg__meta-item">
            <span class="err-dlg__meta-label">{{ t('messageLog.logger') }}</span>
            <span class="err-dlg__meta-value">{{ pickField('logger', 'Logger') }}</span>
          </div>
          <template v-if="pickField('statusCode', 'StatusCode') !== '-'">
            <div class="err-dlg__meta-divider"></div>
            <div class="err-dlg__meta-item">
              <ElTag
                :type="
                  Number(pickField('statusCode', 'StatusCode')) >= 500
                    ? 'danger'
                    : Number(pickField('statusCode', 'StatusCode')) >= 400
                      ? 'warning'
                      : 'info'
                "
                size="small"
                effect="dark"
                >HTTP {{ pickField('statusCode', 'StatusCode') }}</ElTag
              >
            </div>
          </template>
        </div>

        <!-- 异常类型 -->
        <div v-if="hasExceptionType" class="err-dlg__ex-type">
          <span class="err-dlg__ex-type-label">{{ t('messageLog.exceptionType') }}</span>
          <code class="err-dlg__ex-type-name">{{ exceptionType }}</code>
        </div>

        <!-- 错误消息 -->
        <div v-if="hasShortMessage" class="err-dlg__msg">
          <div class="err-dlg__sec-hd">
            <span class="err-dlg__sec-label">{{ t('messageLog.message') }}</span>
            <button class="err-dlg__btn err-dlg__btn--sm" @click="copyMessage">
              {{ t('messageLog.copy') }}
            </button>
          </div>
          <p class="err-dlg__msg-text">{{ shortMessage }}</p>
        </div>

        <!-- 可折叠堆栈 -->
        <div v-if="hasStack" class="err-dlg__stack">
          <button class="err-dlg__stack-btn" @click="toggleStack">
            <span class="err-dlg__stack-btn-left">
              <svg
                class="err-dlg__chevron"
                :class="{ 'err-dlg__chevron--open': stackExpanded }"
                width="14"
                height="14"
                viewBox="0 0 20 20"
                fill="currentColor"
              >
                <path
                  fill-rule="evenodd"
                  d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z"
                  clip-rule="evenodd"
                />
              </svg>
              {{ t('messageLog.exceptionStack') }} ({{ stackLineCount }}
              {{ t('messageLog.stackLayers') }})
            </span>
            <span class="err-dlg__stack-btn-meta">{{ exceptionStack.length }} chars</span>
          </button>
          <pre v-show="stackExpanded" class="err-dlg__pre">{{ exceptionStack }}</pre>
        </div>

        <!-- 异常消息 -->
        <div v-if="exceptionMessage !== '-' && exceptionMessage !== ''" class="err-dlg__ex-msg">
          <span class="err-dlg__sec-label">{{ t('messageLog.exceptionMessage') }}</span>
          <p class="err-dlg__ex-msg-text">{{ exceptionMessage }}</p>
        </div>

        <!-- 补充信息 -->
        <el-descriptions :column="2" border size="small" class="err-dlg__descs">
          <el-descriptions-item :label="t('messageLog.requestPath')" :span="2">
            <span class="err-dlg__mono"
              >[{{ pickField('method', 'Method') }}]
              {{ pickField('requestPath', 'RequestPath') }}</span
            >
          </el-descriptions-item>
          <el-descriptions-item :label="t('messageLog.user')">{{
            pickField('userName', 'UserName')
          }}</el-descriptions-item>
          <el-descriptions-item :label="t('messageLog.clientIp')"
            ><span class="err-dlg__mono">{{
              pickField('clientIp', 'ClientIp')
            }}</span></el-descriptions-item
          >
          <el-descriptions-item :label="t('messageLog.userAgent')" :span="2"
            ><span class="err-dlg__ua">{{
              pickField('userAgent', 'UserAgent')
            }}</span></el-descriptions-item
          >
        </el-descriptions>

        <!-- 底部操作栏 -->
        <div class="err-dlg__footer">
          <button class="err-dlg__btn err-dlg__btn--secondary" @click="copyFullLog">
            {{ t('messageLog.copyLog') }}
          </button>
          <button class="err-dlg__btn err-dlg__btn--danger" @click="detailDialogVisible = false">
            {{ t('common.close') }}
          </button>
        </div>
      </div>
    </ElDialog>
  </ContentWrap>
</template>

<style scoped>
.tab-tag {
  margin-right: 4px;
  vertical-align: middle;
}

.search-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.search-bar__left {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
  align-items: center;
}

.placeholder {
  padding: 60px 0;
}
</style>
