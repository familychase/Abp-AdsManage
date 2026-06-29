<script setup lang="tsx">
import { ContentWrap } from '@/components/ContentWrap'
import { useI18n } from '@/hooks/web/useI18n'
import { Table } from '@/components/Table'
import { ElTag, ElTabs, ElTabPane, ElInput, ElSelect, ElOption } from 'element-plus'
import { ref, reactive, unref, computed, nextTick, watch } from 'vue'
import { useTable } from '@/hooks/web/useTable'
import { CrudSchema } from '@/hooks/web/useCrudSchemas'
import { BaseButton } from '@/components/Button'
import { getAccountListApi } from '@/api/ads/account'
import { getCampaignListApi } from '@/api/ads/campaign'
import { getAdSetListApi } from '@/api/ads/adset'
import { getAdListApi } from '@/api/ads/ad'
import type { AdsAccountDto } from '@/api/ads/account/types'
import type { PlatformType } from '@/api/ads/channel/types'

const { t } = useI18n()

const activeTab = ref('account')

// ══════════════════════════════════════════
//  跨 Tab 选中联动 —— 传递 & 恢复
// ══════════════════════════════════════════
const selectedAccountRows = ref<any[]>([])
const selectedAccountIds = computed(() => selectedAccountRows.value.map((r: any) => r.id))

const selectedCampaignRows = ref<any[]>([])
const selectedCampaignIds = computed(() => selectedCampaignRows.value.map((r: any) => r.campaignId))

const selectedGroupRows = ref<any[]>([])
const selectedGroupIds = computed(() => selectedGroupRows.value.map((r: any) => r.adSetId))

const accountTabLabel = computed(() => {
  const base = t('adManage.tabAccount')
  return selectedAccountIds.value.length > 0 ? `${base} (${selectedAccountIds.value.length})` : base
})

const campaignTabLabel = computed(() => {
  const base = t('adManage.tabCampaign')
  return selectedCampaignIds.value.length > 0
    ? `${base} (${selectedCampaignIds.value.length})`
    : base
})

const groupTabLabel = computed(() => {
  const base = t('adManage.tabAdgroup')
  return selectedGroupIds.value.length > 0 ? `${base} (${selectedGroupIds.value.length})` : base
})

const { tableRegister, tableState, tableMethods } = useTable({
  fetchDataApi: async () => {
    const { currentPage, pageSize } = tableState
    const tab = activeTab.value

    if (tab === 'account') {
      const res = await getAccountListApi({
        page: unref(currentPage),
        pageSize: unref(pageSize),
        filterText: accountFilterText.value || undefined,
        accountNo: accountNoSearch.value || undefined,
        platform:
          accountPlatform.value !== ''
            ? (accountPlatform.value as unknown as PlatformType)
            : undefined,
        accountState: accountState.value !== '' ? (accountState.value as number) : undefined
      })
      return {
        list: res.data.items || [],
        total: res.data.totalCount || 0
      }
    }

    if (tab === 'campaign') {
      const res = await getCampaignListApi({
        page: unref(currentPage),
        pageSize: unref(pageSize),
        campaignName: campaignNameSearch.value || undefined,
        campaignNo: campaignNoSearch.value || undefined,
        accountIds: selectedAccountIds.value.length > 0 ? [...selectedAccountIds.value] : undefined
      })
      return {
        list: res.data.items || [],
        total: res.data.totalCount || 0
      }
    }

    if (tab === 'group') {
      const res = await getAdSetListApi({
        page: unref(currentPage),
        pageSize: unref(pageSize),
        adSetName: adSetNameSearch.value || undefined,
        adSetNo: adSetNoSearch.value || undefined,
        accountIds: selectedAccountIds.value.length > 0 ? [...selectedAccountIds.value] : undefined,
        campaignIds:
          selectedCampaignIds.value.length > 0 ? [...selectedCampaignIds.value] : undefined
      })
      return {
        list: res.data.items || [],
        total: res.data.totalCount || 0
      }
    }

    if (tab === 'creative') {
      const res = await getAdListApi({
        page: unref(currentPage),
        pageSize: unref(pageSize),
        adName: adNameSearch.value || undefined,
        adNo: adNoSearch.value || undefined,
        accountIds: selectedAccountIds.value.length > 0 ? [...selectedAccountIds.value] : undefined,
        campaignIds:
          selectedCampaignIds.value.length > 0 ? selectedCampaignIds.value.map(String) : undefined,
        adSetIds: selectedGroupIds.value.length > 0 ? selectedGroupIds.value.map(String) : undefined
      })
      return {
        list: res.data.items || [],
        total: res.data.totalCount || 0
      }
    }

    return { list: [], total: 0 }
  },
  fetchDelApi: async () => {
    return true
  }
})

const { loading, dataList, total, currentPage, pageSize } = tableState
const { getList, getElTableExpose } = tableMethods

const isRestoring = ref(false)

const onSelectionChange = (rows: any[]) => {
  // 程序化恢复勾选期间跳过，避免 clearSelection/toggleRowSelection 冲掉已保存的选中
  if (isRestoring.value) return
  const tab = activeTab.value
  if (tab === 'account') {
    selectedAccountRows.value = rows
  } else if (tab === 'campaign') {
    selectedCampaignRows.value = rows
  } else if (tab === 'group') {
    selectedGroupRows.value = rows
  }
}

// ══════════════════════════════════════════
//  广告账户 - 搜索条件
// ══════════════════════════════════════════
const accountFilterText = ref('')
const accountNoSearch = ref('')
const accountPlatform = ref<number | ''>('')
const accountState = ref<number | ''>('')

const handleAccountSearch = () => {
  currentPage.value = 1
  getList()
}

const handleAccountReset = () => {
  accountFilterText.value = ''
  accountNoSearch.value = ''
  accountPlatform.value = ''
  accountState.value = ''
  currentPage.value = 1
  getList()
}

// ══════════════════════════════════════════
//  广告系列 - 搜索条件
// ══════════════════════════════════════════
const campaignNameSearch = ref('')
const campaignNoSearch = ref('')

const handleCampaignSearch = () => {
  currentPage.value = 1
  getList()
}

const handleCampaignReset = () => {
  campaignNameSearch.value = ''
  campaignNoSearch.value = ''
  currentPage.value = 1
  getList()
}

// ══════════════════════════════════════════
//  广告组 - 搜索条件
// ══════════════════════════════════════════
const adSetNameSearch = ref('')
const adSetNoSearch = ref('')

const handleAdSetSearch = () => {
  currentPage.value = 1
  getList()
}

const handleAdSetReset = () => {
  adSetNameSearch.value = ''
  adSetNoSearch.value = ''
  currentPage.value = 1
  getList()
}

// ══════════════════════════════════════════
//  广告 - 搜索条件
// ══════════════════════════════════════════
const adNameSearch = ref('')
const adNoSearch = ref('')

const handleAdSearch = () => {
  currentPage.value = 1
  getList()
}

const handleAdReset = () => {
  adNameSearch.value = ''
  adNoSearch.value = ''
  currentPage.value = 1
  getList()
}

// Tab 标签

// Account schemas
const accountSchemas = reactive<CrudSchema[]>([
  {
    field: 'selection_account',
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true },
    table: { type: 'selection' }
  },
  {
    field: 'index',
    label: t('tableDemo.index'),
    type: 'index',
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true }
  },
  {
    field: 'accountNo',
    label: t('accountManage.accountNo'),
    search: { hidden: true },
    table: {
      minWidth: '200px',
      slots: {
        default: (d: any) => (
          <span style="font-family: monospace; font-size: 13px">{d.row.accountNo || '-'}</span>
        )
      }
    }
  },
  {
    field: 'accountName',
    label: t('accountManage.accountName'),
    search: { hidden: true },
    table: {
      minWidth: '180px',
      slots: {
        default: (d: any) => (
          <div style="font-weight: 500; color: #409eff">{d.row.accountName || '-'}</div>
        )
      }
    }
  },
  {
    field: 'platform',
    label: t('accountManage.platform'),
    search: { hidden: true },
    table: {
      minWidth: '120px',
      slots: {
        default: (d: any) => {
          const plt = Number(d.row.platform)
          const colorMap: Record<number, string> = {
            0: '#909399',
            1: '#4285f4',
            2: '#1877f2',
            3: '#000'
          }
          const labelMap: Record<number, string> = {
            0: '未知',
            1: 'Google',
            2: 'Facebook',
            3: 'TikTok'
          }
          return (
            <ElTag
              style={{ border: 'none', color: '#fff', backgroundColor: colorMap[plt] || '#909399' }}
              size="small"
            >
              {labelMap[plt] || d.row.platform}
            </ElTag>
          )
        }
      }
    }
  },
  {
    field: 'accountState',
    label: t('accountManage.accountState'),
    search: { hidden: true },
    table: {
      minWidth: '100px',
      slots: {
        default: (d: any) => {
          const row = d.row as AdsAccountDto
          const isNormal = row.accountState === 1 || String(row.accountState) === 'NORMAL'
          return (
            <ElTag type={isNormal ? 'success' : 'danger'}>
              {isNormal ? t('common.normal') : t('common.abnormal')}
            </ElTag>
          )
        }
      }
    }
  },
  {
    field: 'balance',
    label: t('accountManage.balance'),
    search: { hidden: true },
    table: {
      minWidth: '130px',
      slots: {
        default: (d: any) => {
          const val = Number(d.row.balance) || 0
          const isLow = val < 10000
          return (
            <span style={{ color: isLow ? '#f56c6c' : '#67c23a', fontWeight: 500 }}>
              $ {val.toLocaleString('en-US', { minimumFractionDigits: 2 })}
            </span>
          )
        }
      }
    }
  },
  {
    field: 'timezone',
    label: t('accountManage.timezone'),
    search: { hidden: true },
    table: {
      minWidth: '160px',
      slots: {
        default: (data: any) => {
          const row: AdsAccountDto = data.row
          const tz = row.timezone
          const offset = row.utcTimezoneOffset
          return (
            <span style="font-size: 12px">
              {tz || '-'}
              {tz && offset && <span style="color: #409eff">({offset})</span>}
            </span>
          )
        }
      }
    }
  },
  {
    field: 'creationTime',
    label: t('accountManage.creationTime'),
    search: { hidden: true },
    table: { minWidth: '160px' }
  }
])

// Campaign schemas
const campaignSchemas = reactive<CrudSchema[]>([
  {
    field: 'selection_campaign',
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true },
    table: { type: 'selection' }
  },
  {
    field: 'index',
    label: t('tableDemo.index'),
    type: 'index',
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true }
  },
  {
    field: 'campaignNo',
    label: t('adManage.campaignId'),
    search: { hidden: true },
    table: {
      minWidth: '120px',
      slots: {
        default: (d: any) => (
          <span style="font-family: monospace; font-size: 13px">{d.row.campaignNo || '-'}</span>
        )
      }
    }
  },
  {
    field: 'campaignName',
    label: t('adManage.campaignName'),
    search: { hidden: true },
    table: { minWidth: '200px' }
  },
  {
    field: 'accountNo',
    label: t('accountManage.accountNo'),
    search: { hidden: true }
  },
  {
    field: 'objective',
    label: t('adManage.objective'),
    search: { hidden: true },
    table: {
      slots: {
        default: (d: any) => {
          const label = d.row.objective || '-'
          return <span>{label}</span>
        }
      }
    }
  },
  {
    field: 'budget',
    label: t('adManage.budget'),
    search: { hidden: true },
    table: {
      slots: {
        default: (d: any) => {
          const val = Number(d.row.budget) || 0
          return <span>$ {val.toLocaleString('en-US', { minimumFractionDigits: 2 })}</span>
        }
      }
    }
  },
  {
    field: 'mediaState',
    label: t('userDemo.status'),
    search: { hidden: true },
    table: {
      slots: {
        default: (d: any) => {
          const state = (d.row.mediaState || '').toUpperCase()
          const stateMap: Record<
            string,
            { type: 'success' | 'warning' | 'info' | 'danger'; label: string }
          > = {
            ACTIVE: { type: 'success', label: t('adManage.enabled') },
            PAUSED: { type: 'warning', label: t('adManage.paused') },
            DISABLED: { type: 'info', label: t('adManage.disabled') }
          }
          const cfg = stateMap[state] || { type: 'danger', label: state }
          return <ElTag type={cfg.type}>{cfg.label}</ElTag>
        }
      }
    }
  },
  {
    field: 'creationTime',
    label: t('adManage.createTime'),
    search: { hidden: true },
    table: {
      slots: {
        default: (d: any) => {
          const t = d.row.creationTime
          return <span>{t ? t.split('T')[0] : '-'}</span>
        }
      }
    }
  }
])

// Ad group schemas
const adgroupSchemas = reactive<CrudSchema[]>([
  {
    field: 'selection_adgroup',
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true },
    table: { type: 'selection' }
  },
  {
    field: 'index',
    label: t('tableDemo.index'),
    type: 'index',
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true }
  },
  {
    field: 'adSetNo',
    label: t('adManage.adgroupId'),
    search: { hidden: true },
    table: {
      minWidth: '120px',
      slots: {
        default: (d: any) => (
          <span style="font-family: monospace; font-size: 13px">{d.row.adSetNo || '-'}</span>
        )
      }
    }
  },
  {
    field: 'adSetName',
    label: t('adManage.adgroupName'),
    search: { hidden: true },
    table: { minWidth: '200px' }
  },
  { field: 'campaignNo', label: t('adManage.belongCampaign'), search: { hidden: true } },
  { field: 'accountNo', label: t('accountManage.accountNo'), search: { hidden: true } },
  {
    field: 'budget',
    label: t('adManage.budget'),
    search: { hidden: true },
    table: {
      slots: {
        default: (d: any) => {
          const val = Number(d.row.budget) || 0
          return <span>$ {val.toLocaleString('en-US', { minimumFractionDigits: 2 })}</span>
        }
      }
    }
  },
  {
    field: 'mediaState',
    label: t('userDemo.status'),
    search: { hidden: true },
    table: {
      slots: {
        default: (d: any) => {
          const state = (d.row.mediaState || '').toUpperCase()
          const stateMap: Record<
            string,
            { type: 'success' | 'warning' | 'info' | 'danger'; label: string }
          > = {
            ACTIVE: { type: 'success', label: t('adManage.enabled') },
            PAUSED: { type: 'warning', label: t('adManage.paused') },
            DISABLED: { type: 'info', label: t('adManage.disabled') }
          }
          const cfg = stateMap[state] || { type: 'danger', label: state }
          return <ElTag type={cfg.type}>{cfg.label}</ElTag>
        }
      }
    }
  },
  {
    field: 'creationTime',
    label: t('adManage.createTime'),
    search: { hidden: true },
    table: {
      slots: {
        default: (d: any) => {
          const t = d.row.creationTime
          return <span>{t ? t.split('T')[0] : '-'}</span>
        }
      }
    }
  }
])

// Creative schemas
const creativeSchemas = reactive<CrudSchema[]>([
  {
    field: 'index',
    label: t('tableDemo.index'),
    type: 'index',
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true }
  },
  {
    field: 'adNo',
    label: t('adManage.adNo'),
    search: { hidden: true },
    table: {
      minWidth: '120px',
      slots: {
        default: (d: any) => (
          <span style="font-family: monospace; font-size: 13px">{d.row.adNo || '-'}</span>
        )
      }
    }
  },
  {
    field: 'adName',
    label: t('adManage.adName'),
    search: { hidden: true },
    table: { minWidth: '200px' }
  },
  { field: 'campaignNo', label: t('adManage.belongCampaign'), search: { hidden: true } },
  { field: 'adSetNo', label: t('adManage.belongAdSet'), search: { hidden: true } },
  { field: 'accountNo', label: t('accountManage.accountNo'), search: { hidden: true } },
  { field: 'creativeNo', label: t('adManage.creativeNo'), search: { hidden: true } },
  {
    field: 'mediaState',
    label: t('userDemo.status'),
    search: { hidden: true },
    table: {
      slots: {
        default: (d: any) => {
          const state = (d.row.mediaState || '').toUpperCase()
          const stateMap: Record<
            string,
            { type: 'success' | 'warning' | 'info' | 'danger'; label: string }
          > = {
            ACTIVE: { type: 'success', label: t('adManage.enabled') },
            PAUSED: { type: 'warning', label: t('adManage.paused') },
            DISABLED: { type: 'info', label: t('adManage.disabled') }
          }
          const cfg = stateMap[state] || { type: 'danger', label: state }
          return <ElTag type={cfg.type}>{cfg.label}</ElTag>
        }
      }
    }
  },
  {
    field: 'creationTime',
    label: t('adManage.createTime'),
    search: { hidden: true },
    table: {
      slots: {
        default: (d: any) => {
          const t = d.row.creationTime
          return <span>{t ? t.split('T')[0] : '-'}</span>
        }
      }
    }
  }
])

// Active schema
const activeSchemas = computed(() => {
  const map: Record<string, any> = {
    account: accountSchemas,
    campaign: campaignSchemas,
    group: adgroupSchemas,
    creative: creativeSchemas
  }
  return map[activeTab.value] || accountSchemas
})

const tableColumns = computed(() => {
  const schemas = activeSchemas.value
  return schemas
    .filter((s: CrudSchema) => !s.table?.hidden)
    .map((s: CrudSchema, idx: number) => {
      const { search, form, detail, children, ...rest } = s
      return { ...rest, ...s.table, key: s.field || `col-${idx}` }
    })
})

const handleTabChange = () => {
  currentPage.value = 1
  getList()
}

const rowKeyField = computed(() => {
  const tab = activeTab.value
  if (tab === 'campaign') return 'campaignId'
  if (tab === 'group') return 'adSetId'
  if (tab === 'creative') return 'adId'
  return 'id'
})

// ══════════════════════════════════════════
//  切回 tab 时恢复勾选状态
// ══════════════════════════════════════════
watch([() => activeTab.value, () => dataList.value], async ([tab, list]) => {
  if (!list || list.length === 0) return

  let targetRows: any[] = []
  let idField = 'id'

  if (tab === 'account') {
    targetRows = selectedAccountRows.value
    idField = 'id'
  } else if (tab === 'campaign') {
    targetRows = selectedCampaignRows.value
    idField = 'campaignId'
  } else if (tab === 'group') {
    targetRows = selectedGroupRows.value
    idField = 'adSetId'
  }

  await nextTick()
  const elRef = await getElTableExpose()
  if (!elRef) return

  // 程序化操作期间屏蔽 selection-change，防止 clearSelection / toggleRowSelection 冲掉已保存的选中
  isRestoring.value = true

  // 每次数据刷新先清空表格选中，再按需恢复当前 Tab 的勾选，避免 Tab 间残留
  elRef.clearSelection()

  if (targetRows.length > 0) {
    const idSet = new Set(targetRows.map((r: any) => r[idField]))
    ;(list as any[]).forEach((row: any) => {
      if (idSet.has(row[idField])) {
        elRef.toggleRowSelection(row, true)
      }
    })
  }

  // 延迟关闭标志，确保所有 toggleRowSelection 触发的 selection-change 都被拦截
  await nextTick()
  isRestoring.value = false
})
</script>

<template>
  <ContentWrap>
    <ElTabs v-model="activeTab" @tab-change="handleTabChange">
      <ElTabPane :label="accountTabLabel" name="account" />
      <ElTabPane :label="campaignTabLabel" name="campaign" />
      <ElTabPane :label="groupTabLabel" name="group" />
      <ElTabPane :label="t('adManage.tabCreative')" name="creative" />
    </ElTabs>

    <!-- 广告账户搜索栏 -->
    <div
      v-if="activeTab === 'account'"
      class="mb-10px flex gap-10px"
      style="display: flex; gap: 12px; flex-wrap: wrap; align-items: center; margin-bottom: 20px"
    >
      <ElInput
        v-model="accountFilterText"
        :placeholder="t('accountManage.searchPlaceholder')"
        clearable
        style="width: 200px"
        @keyup.enter="handleAccountSearch"
        @clear="handleAccountSearch"
      />
      <ElSelect
        v-model="accountPlatform"
        :placeholder="t('accountManage.platform')"
        clearable
        style="width: 140px"
        @change="handleAccountSearch"
      >
        <ElOption label="Google" :value="1" />
        <ElOption label="Facebook" :value="2" />
        <ElOption label="TikTok" :value="3" />
      </ElSelect>
      <ElSelect
        v-model="accountState"
        :placeholder="t('accountManage.accountState')"
        clearable
        style="width: 140px"
        @change="handleAccountSearch"
      >
        <ElOption :label="t('common.normal')" :value="1" />
        <ElOption :label="t('common.abnormal')" :value="2" />
      </ElSelect>
      <ElInput
        v-model="accountNoSearch"
        :placeholder="t('accountManage.accountNo')"
        clearable
        style="width: 160px"
        @keyup.enter="handleAccountSearch"
        @clear="handleAccountSearch"
      />
      <BaseButton type="primary" @click="handleAccountSearch">{{ t('common.query') }}</BaseButton>
      <BaseButton @click="handleAccountReset">{{ t('common.reset') }}</BaseButton>
    </div>

    <!-- 广告系列搜索栏 -->
    <div
      v-if="activeTab === 'campaign'"
      class="mb-10px flex gap-10px"
      style="display: flex; gap: 12px; flex-wrap: wrap; align-items: center; margin-bottom: 20px"
    >
      <ElInput
        v-model="campaignNameSearch"
        :placeholder="t('adManage.campaignName')"
        clearable
        style="width: 200px"
        @keyup.enter="handleCampaignSearch"
        @clear="handleCampaignSearch"
      />
      <ElInput
        v-model="campaignNoSearch"
        :placeholder="t('adManage.campaignId')"
        clearable
        style="width: 160px"
        @keyup.enter="handleCampaignSearch"
        @clear="handleCampaignSearch"
      />
      <BaseButton type="primary" @click="handleCampaignSearch">{{ t('common.query') }}</BaseButton>
      <BaseButton @click="handleCampaignReset">{{ t('common.reset') }}</BaseButton>
    </div>

    <!-- 广告组搜索栏 -->
    <div
      v-if="activeTab === 'group'"
      class="mb-10px flex gap-10px"
      style="display: flex; gap: 12px; flex-wrap: wrap; align-items: center; margin-bottom: 20px"
    >
      <ElInput
        v-model="adSetNameSearch"
        :placeholder="t('adManage.adgroupName')"
        clearable
        style="width: 200px"
        @keyup.enter="handleAdSetSearch"
        @clear="handleAdSetSearch"
      />
      <ElInput
        v-model="adSetNoSearch"
        :placeholder="t('adManage.adgroupId')"
        clearable
        style="width: 160px"
        @keyup.enter="handleAdSetSearch"
        @clear="handleAdSetSearch"
      />
      <BaseButton type="primary" @click="handleAdSetSearch">{{ t('common.query') }}</BaseButton>
      <BaseButton @click="handleAdSetReset">{{ t('common.reset') }}</BaseButton>
    </div>

    <!-- 广告搜索栏 -->
    <div
      v-if="activeTab === 'creative'"
      class="mb-10px flex gap-10px"
      style="display: flex; gap: 12px; flex-wrap: wrap; align-items: center; margin-bottom: 20px"
    >
      <ElInput
        v-model="adNameSearch"
        :placeholder="t('adManage.adName')"
        clearable
        style="width: 200px"
        @keyup.enter="handleAdSearch"
        @clear="handleAdSearch"
      />
      <ElInput
        v-model="adNoSearch"
        :placeholder="t('adManage.adNo')"
        clearable
        style="width: 160px"
        @keyup.enter="handleAdSearch"
        @clear="handleAdSearch"
      />
      <BaseButton type="primary" @click="handleAdSearch">{{ t('common.query') }}</BaseButton>
      <BaseButton @click="handleAdReset">{{ t('common.reset') }}</BaseButton>
    </div>

    <Table
      v-model:pageSize="pageSize"
      v-model:currentPage="currentPage"
      :columns="tableColumns"
      :data="dataList"
      :loading="loading"
      :pagination="{ total }"
      :row-key="rowKeyField"
      :reserve-selection="activeTab !== 'creative'"
      @selection-change="onSelectionChange"
      @register="tableRegister"
    />
  </ContentWrap>
</template>
