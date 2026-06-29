<script setup lang="tsx">
import { ContentWrap } from '@/components/ContentWrap'
import { useI18n } from '@/hooks/web/useI18n'
import { Table } from '@/components/Table'
import {
  ElTag,
  ElSelect,
  ElOption,
  ElInput,
  ElMessage,
  ElDialog,
  ElTable,
  ElTableColumn
} from 'element-plus'
import { ref, reactive, unref, onMounted } from 'vue'
import { useTable } from '@/hooks/web/useTable'
import { CrudSchema, useCrudSchemas } from '@/hooks/web/useCrudSchemas'
import { BaseButton } from '@/components/Button'

import { getChannelListApi, deleteChannelApi } from '@/api/ads/channel'
import type {
  AdsChannelDto,
  GetAdsChannelListInput,
  PlatformType,
  ChannelStateType
} from '@/api/ads/channel/types'
import { PLATFORM_LABELS } from '@/constants/platform'
import { metaOAuthCallbackApi } from '@/api/ads/metaOAuth'
import {
  createSyncScheduleApi,
  getSyncScheduleListApi,
  pushSyncScheduleApi
} from '@/api/ads/syncSchedule'

const { t } = useI18n()

// ══════════════════════════════════════════
//  平台 / 状态 映射
// ══════════════════════════════════════════

const platformColorMap: Record<PlatformType, string> = {
  0: '#909399',
  1: '#4285f4',
  2: '#1877f2',
  3: '#000'
}

// ══════════════════════════════════════════
//  查询条件
// ══════════════════════════════════════════

const searchKeyword = ref('')
const searchPlatform = ref<PlatformType | ''>('')
const searchState = ref<ChannelStateType | ''>('')

const getSearchParams = (): GetAdsChannelListInput => ({
  page: unref(currentPage),
  pageSize: unref(pageSize),
  filterText: searchKeyword.value || undefined,
  platform: searchPlatform.value !== '' ? (searchPlatform.value as PlatformType) : undefined,
  channelState: searchState.value !== '' ? (searchState.value as ChannelStateType) : undefined
})

// ══════════════════════════════════════════
//  表格 & 数据
// ══════════════════════════════════════════

const ids = ref<string[]>([])

const { tableRegister, tableState, tableMethods } = useTable({
  fetchDataApi: async () => {
    const res = await getChannelListApi(getSearchParams())
    const items = res.data.items || []
    return {
      list: items,
      total: res.data.totalCount || 0
    }
  },
  fetchDelApi: async () => {
    const idArr = unref(ids)
    if (idArr.length === 0) return false
    await deleteChannelApi(Number(idArr[0]))
    return true
  }
})

const { loading, dataList, total, currentPage, pageSize } = tableState
const { getList, delList } = tableMethods

const handleSearch = () => {
  currentPage.value = 1
  getList()
}

const handleReset = () => {
  searchKeyword.value = ''
  searchPlatform.value = ''
  searchState.value = ''
  currentPage.value = 1
  getList()
}

// ══════════════════════════════════════════
//  列定义
// ══════════════════════════════════════════

const crudSchemas = reactive<CrudSchema[]>([
  {
    field: 'selection',
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true },
    table: { type: 'selection', width: '50px' }
  },
  {
    field: 'channelName',
    label: t('auth.channelName'),
    search: { hidden: true },
    table: {
      minWidth: '180px',
      slots: {
        default: (data: any) => {
          const row: AdsChannelDto = data.row
          return <div style="font-weight: 500">{row.channelName}</div>
        }
      }
    }
  },
  {
    field: 'platform',
    label: t('auth.platform'),
    search: { hidden: true },
    table: {
      width: '130px',
      slots: {
        default: (data: any) => {
          const row: AdsChannelDto = data.row
          const plt = Number(row.platform)
          return (
            <ElTag
              style={{
                border: 'none',
                color: '#fff',
                backgroundColor: platformColorMap[plt] || '#909399'
              }}
              size="small"
            >
              {PLATFORM_LABELS[plt] || row.platform}
            </ElTag>
          )
        }
      }
    }
  },
  {
    field: 'channelState',
    label: t('auth.channelState'),
    search: { hidden: true },
    table: {
      width: '100px',
      slots: {
        default: (data: any) => {
          const row: AdsChannelDto = data.row
          const isActive = row.channelState === 1 || String(row.channelState) === 'ACTIVE'
          const cfg = isActive
            ? { type: 'success' as const, label: t('common.normal') }
            : { type: 'danger' as const, label: t('common.abnormal') }
          return <ElTag type={cfg.type}>{cfg.label}</ElTag>
        }
      }
    }
  },
  {
    field: 'action',
    width: '100px',
    label: t('tableDemo.action'),
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true },
    table: {
      fixed: 'right',
      width: '180px',
      slots: {
        default: (data: any) => (
          <>
            <BaseButton type="primary" size="small" onClick={() => openSyncDialog(data.row)}>
              {t('auth.syncBtn')}
            </BaseButton>
            <BaseButton type="danger" size="small" onClick={() => delData(data.row)}>
              {t('exampleDemo.del')}
            </BaseButton>
          </>
        )
      }
    }
  }
])

const { allSchemas } = useCrudSchemas(crudSchemas)

const delData = async (row: AdsChannelDto) => {
  ids.value = [String(row.id)]
  await delList(1)
}

// ══════════════════════════════════════════
//  同步弹窗
// ══════════════════════════════════════════

const syncDialogVisible = ref(false)
const syncChannelRef = ref<AdsChannelDto | null>(null)

const syncDataList = ref<
  Array<{ id?: number; jobName?: string; scheduleKey?: string; name?: string }>
>([])
const syncLoading = ref(false)

const openSyncDialog = async (row: AdsChannelDto) => {
  syncChannelRef.value = row
  syncDialogVisible.value = true
  syncLoading.value = true
  try {
    const res = await getSyncScheduleListApi({
      resourceType: 'CHANNEL',
      resourceId: String(row.id)
    })
    syncDataList.value = ((res.data as any)?.items || res.data || []) as any[]
  } catch {
    syncDataList.value = []
    ElMessage.warning(t('auth.syncLoadFailed'))
  } finally {
    syncLoading.value = false
  }
}

const handleSyncItem = async (item: { id?: number; name?: string; jobName?: string }) => {
  const row = syncChannelRef.value
  if (!row) return

  try {
    if (item.id) {
      await pushSyncScheduleApi(item.id)
    } else {
      await createSyncScheduleApi({
        actionType: 0,
        jobName: item.jobName || item.name || `${row.channelName} - 同步`,
        platform: row.platform,
        resourceId: String(row.id),
        resourceType: 'CHANNEL',
        nextPublishTime: new Date().toISOString()
      })
    }
    ElMessage.success(t('auth.syncSubmitSuccess'))
  } catch (err: any) {
    ElMessage.error(t('auth.syncFailed') + (err?.message || ''))
  }
}

// ══════════════════════════════════════════
//  新增授权 → 弹窗 OAuth → postMessage 回调父窗口
// ══════════════════════════════════════════

const FACEBOOK_APP_ID = import.meta.env.VITE_FACEBOOK_APP_ID || ''
const META_CONFIG_ID = import.meta.env.VITE_META_CONFIG_ID || ''
const OAUTH_REDIRECT_URI =
  window.location.origin + import.meta.env.BASE_URL + 'oauth-meta-callback.html'

/** 处理 OAuth 回调（提取 code/state → 调后端） */
const executeOAuthExchange = (code: string, receivedState: string) => {
  const savedState = sessionStorage.getItem('meta_oauth_state')
  if (receivedState !== savedState) {
    sessionStorage.removeItem('meta_oauth_state')
    return
  }
  sessionStorage.removeItem('meta_oauth_state')

  metaOAuthCallbackApi({
    code,
    appId: FACEBOOK_APP_ID,
    redirectUri: OAUTH_REDIRECT_URI,
    email: null,
    isManager: false
  })
    .then(() => {
      ElMessage.success(t('auth.addSuccess'))
      getList()
    })
    .catch((err: any) => {
      ElMessage.error(t('auth.authFailed') + (err?.message || ''))
    })
}

const addAuth = () => {
  const state = crypto.randomUUID()
  sessionStorage.setItem('meta_oauth_state', state)

  const redirectUri = OAUTH_REDIRECT_URI
  const oauthUrl =
    `https://www.facebook.com/v25.0/dialog/oauth` +
    `?client_id=${FACEBOOK_APP_ID}` +
    `&config_id=${META_CONFIG_ID}` +
    `&redirect_uri=${encodeURIComponent(redirectUri)}` +
    `&state=${state}` +
    `&auth_type=rerequest` +
    `&response_type=code`

  // 监听子窗口 postMessage 回调
  const messageHandler = (event: MessageEvent) => {
    // 安全检查：只接受同源消息
    if (event.origin !== window.location.origin) return
    const payload = event.data
    if (!payload || payload.type !== 'META_OAUTH_CALLBACK') return

    const { code, state: receivedState } = payload
    if (!code || !receivedState) return

    window.removeEventListener('message', messageHandler)
    executeOAuthExchange(code, receivedState)
  }

  window.addEventListener('message', messageHandler)

  const sw = screen.width
  const sh = screen.height
  const pw = 900
  const ph = 700
  const left = Math.round((sw - pw) / 2)
  const top = Math.round((sh - ph) / 2)
  const popup = window.open(oauthUrl, 'fb_auth', `width=${pw},height=${ph},left=${left},top=${top}`)
  if (!popup) {
    ElMessage.warning(t('auth.popupBlocked'))
    window.removeEventListener('message', messageHandler)
    return
  }

  // 兜底：用户取消授权 / 弹窗意外关闭时清理监听
  const timer = setInterval(() => {
    if (popup.closed) {
      clearInterval(timer)
      window.removeEventListener('message', messageHandler)
    }
  }, 500)
}

// 兼容非弹窗场景：页面直接刷新后仍有 code 参数（非 window.opener 时处理）
const handleOAuthCallback = () => {
  const params = new URLSearchParams(window.location.search)
  const code = params.get('code')
  const state = params.get('state')
  if (!code || !state) return

  const url = new URL(window.location.href)
  url.searchParams.delete('code')
  url.searchParams.delete('state')
  window.history.replaceState({}, '', url.toString())

  executeOAuthExchange(code, state)
}

onMounted(() => {
  // 兼容场景：页面直接跳转（如浏览器书签 / 非弹窗流程）
  handleOAuthCallback()
})
</script>

<template>
  <ContentWrap>
    <!-- 搜索栏 -->
    <div
      style="
        display: flex;
        justify-content: space-between;
        align-items: flex-start;
        flex-wrap: wrap;
        gap: 12px;
        margin-bottom: 20px;
      "
    >
      <div style="display: flex; align-items: center; flex-wrap: wrap; gap: 12px">
        <ElInput
          v-model="searchKeyword"
          :placeholder="t('auth.keywordPlaceholder')"
          clearable
          style="width: 220px"
          @keyup.enter="handleSearch"
          @clear="handleSearch"
        />
        <ElSelect
          v-model="searchPlatform"
          :placeholder="t('auth.platform')"
          clearable
          style="width: 160px"
          @change="handleSearch"
        >
          <ElOption
            v-for="(label, val) in PLATFORM_LABELS"
            :key="val"
            :label="label"
            :value="Number(val)"
          />
        </ElSelect>
        <ElSelect
          v-model="searchState"
          :placeholder="t('auth.channelState')"
          clearable
          style="width: 160px"
          @change="handleSearch"
        >
          <ElOption :label="t('common.normal')" :value="1" />
          <ElOption :label="t('common.abnormal')" :value="0" />
        </ElSelect>
        <BaseButton type="primary" @click="handleSearch">{{ t('common.query') }}</BaseButton>
        <BaseButton @click="handleReset">{{ t('common.reset') }}</BaseButton>
      </div>
      <div>
        <BaseButton type="success" @click="addAuth">{{ t('auth.addBtn') }}</BaseButton>
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

    <!-- 同步数据弹窗 -->
    <ElDialog v-model="syncDialogVisible" :title="t('auth.syncTitle')" width="540px" top="15vh">
      <ElTable v-loading="syncLoading" :data="syncDataList" border stripe>
        <ElTableColumn type="index" label="#" width="60" />
        <ElTableColumn prop="jobNameDisplay" :label="t('common.name')" />
        <ElTableColumn :label="t('auth.syncAction')" width="120">
          <template #default="{ row }">
            <BaseButton type="primary" size="small" @click="handleSyncItem(row)">
              {{ t('auth.syncNow') }}
            </BaseButton>
          </template>
        </ElTableColumn>
      </ElTable>
      <template #footer>
        <BaseButton @click="syncDialogVisible = false">{{ t('common.close') }}</BaseButton>
      </template>
    </ElDialog>
  </ContentWrap>
</template>
