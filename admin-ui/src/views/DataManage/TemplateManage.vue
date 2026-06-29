<script setup lang="tsx">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { ContentWrap } from '@/components/ContentWrap'
import { Table } from '@/components/Table'
import { BaseButton } from '@/components/Button'
import {
  ElInput,
  ElSelect,
  ElOption,
  ElMessage,
  ElMessageBox,
  ElDropdown,
  ElDropdownMenu,
  ElDropdownItem,
  ElDatePicker,
  ElButton,
  ElTag
} from 'element-plus'
import { useTable } from '@/hooks/web/useTable'
import { CrudSchema, useCrudSchemas } from '@/hooks/web/useCrudSchemas'
import { useI18n } from '@/hooks/web/useI18n'
import { PLATFORM_OPTIONS, PLATFORM_LABELS } from '@/constants/platform'
import type { PlatformType } from '@/api/ads/channel/types'

// ══════════════════════════════════════════
//  API
// ══════════════════════════════════════════

import {
  getTemplateListApi,
  removeTemplateApi,
  reproductionTemplateApi,
  getTemplateDetailApi
} from '@/api/ads/template'
import type { TemplateDto, AdsPublishTemplateItem } from '@/api/ads/template/types'

// ══════════════════════════════════════════
//  组件
// ══════════════════════════════════════════

import TemplateEditorDrawer from './components/TemplateEditorDrawer.vue'

const { t } = useI18n()
const router = useRouter()

// ══════════════════════════════════════════
//  查询条件
// ══════════════════════════════════════════

const searchTemplateName = ref('')
const searchTimeRange = ref<[string, string] | undefined>(undefined)
const searchPlatform = ref<number | ''>('')
const searchCreator = ref('')
const searchPixelId = ref<number | undefined>(undefined)

// ══════════════════════════════════════════
//  表格
// ══════════════════════════════════════════

/** 模板行类型（新 API 字段映射） */
interface TemplateRow extends TemplateDto {
  version: string
  resourceId: number
  publishAdCount: number
  lastPublishTime: string | null
}

const { tableRegister, tableState, tableMethods } = useTable({
  fetchDataApi: async () => {
    const res = await getTemplateListApi({
      page: tableState.currentPage.value - 1,
      pageSize: tableState.pageSize.value,
      name: searchTemplateName.value || undefined,
      platform: (searchPlatform.value ? Number(searchPlatform.value) : 0) as PlatformType,
      application_id: undefined,
      pixel_id: searchPixelId.value || undefined,
      creator_id: searchCreator.value ? Number(searchCreator.value) : undefined,
      ...(searchTimeRange.value?.[0] && searchTimeRange.value?.[1]
        ? {
            dateRange: {
              start: new Date(searchTimeRange.value[0]).toISOString(),
              stop: new Date(searchTimeRange.value[1]).toISOString()
            }
          }
        : {})
    })
    const list: TemplateRow[] = (res.data?.items ?? []).map((item: AdsPublishTemplateItem) => ({
      id: item.id,
      templateId: item.id,
      templateName: item.name,
      platform: item.platform,
      publishingAdType: item.publishingAdType as any,
      publishStatus: 'PUBLISHED' as any,
      successCount: 0,
      plannedCount: 0,
      adAccountCount: 0,
      campaignCount: 0,
      creator: item.creatorName || '-',
      createdTime: item.creationTime,
      version: item.version,
      resourceId: item.resourceId,
      publishAdCount: item.publishAdCount,
      lastPublishTime: item.lastPublishTime
    }))
    return { list, total: res.data?.totalCount ?? 0 }
  }
})

function refreshList() {
  tableState.currentPage.value = 1
  tableMethods.getList()
}

// ══════════════════════════════════════════
//  搜索
// ══════════════════════════════════════════

const handleSearch = () => {
  refreshList()
}

const handleReset = () => {
  searchTemplateName.value = ''
  searchTimeRange.value = undefined
  searchPlatform.value = ''
  searchCreator.value = ''
  searchPixelId.value = undefined
  handleSearch()
}

// ══════════════════════════════════════════
//  模板编辑器
// ══════════════════════════════════════════

const editorVisible = ref(false)
const editingTemplateId = ref<number | undefined>(undefined)
const initialPlatform = ref<PlatformType>(2)

function handleCreate() {
  editingTemplateId.value = undefined
  initialPlatform.value = searchPlatform.value ? (Number(searchPlatform.value) as PlatformType) : 2
  editorVisible.value = true
}

function handleEdit(row: TemplateDto) {
  editingTemplateId.value = row.id
  editorVisible.value = true
}

function handleEditorSuccess() {
  editorVisible.value = false
  refreshList()
}

// ══════════════════════════════════════════
//  CRUD 操作
// ══════════════════════════════════════════

async function handleRemove(row: TemplateDto) {
  try {
    await ElMessageBox.confirm(
      t('templateManage.confirmDelete', { name: row.templateName }),
      t('common.delWarning'),
      {
        confirmButtonText: t('common.delOk'),
        cancelButtonText: t('common.delCancel'),
        type: 'warning'
      }
    )
  } catch {
    return
  }
  try {
    await removeTemplateApi(row.id)
    ElMessage.success(t('templateManage.deleteSuccess'))
    refreshList()
  } catch (err: any) {
    ElMessage.error(err?.message || t('common.operationFailed'))
  }
}

async function handleReproduction(row: TemplateDto) {
  try {
    await ElMessageBox.confirm(
      t('templateManage.confirmReproduction', { name: row.templateName }),
      t('common.tipsText')
    )
  } catch {
    return
  }
  try {
    await reproductionTemplateApi(row.id)
    ElMessage.success(t('common.successfulOperation'))
    refreshList()
  } catch (err: any) {
    ElMessage.error(err?.message || t('common.operationFailed'))
  }
}

async function handlePublishWithTemplate(row: TemplateDto) {
  router.push({
    path: '/data-manage/publish-manage',
    query: { templateId: String(row.id), platform: String(row.platform) }
  })
}

async function handleViewDetail(row: TemplateDto) {
  try {
    const res = await getTemplateDetailApi(row.id)
    ElMessage.info(JSON.stringify(res.data, null, 2))
  } catch (err) {
    ElMessage.error('获取详情失败')
  }
}

// ══════════════════════════════════════════
//  列定义
// ══════════════════════════════════════════

function createColumns(): CrudSchema[] {
  return [
    {
      field: 'selection',
      label: '',
      type: 'selection' as any,
      search: { hidden: true },
      form: { hidden: true },
      detail: { hidden: true },
      table: { width: '50px' }
    },
    {
      field: 'templateInfo',
      label: t('templateManage.templateInfo'),
      search: { hidden: true },
      table: {
        minWidth: '280px',
        slots: {
          default: (data: any) => {
            const row: TemplateRow = data.row as TemplateRow
            return (
              <div style="display:flex;align-items:center;gap:12px">
                <div class="tm-icon">
                  <svg width="22" height="22" viewBox="0 0 24 24" fill="none">
                    <rect
                      x="3"
                      y="3"
                      width="7"
                      height="7"
                      rx="1.5"
                      fill="var(--el-color-primary)"
                    />
                    <rect
                      x="14"
                      y="3"
                      width="7"
                      height="7"
                      rx="1.5"
                      fill="var(--el-color-primary-light-5)"
                    />
                    <rect
                      x="3"
                      y="14"
                      width="7"
                      height="7"
                      rx="1.5"
                      fill="var(--el-color-primary-light-5)"
                    />
                    <rect
                      x="14"
                      y="14"
                      width="7"
                      height="7"
                      rx="1.5"
                      fill="var(--el-color-primary)"
                    />
                  </svg>
                </div>
                <div class="tm-info">
                  <span class="tm-name">{row.templateName}</span>
                  <span class="tm-sub">
                    v{row.version}
                    {row.resourceId ? ` · 资源#${row.resourceId}` : ''}
                  </span>
                </div>
              </div>
            )
          }
        }
      }
    },
    {
      field: 'platform',
      label: t('templateManage.mediaPlatform'),
      search: { hidden: true },
      table: {
        width: '100px',
        slots: {
          default: (data: any) => {
            const row: TemplateRow = data.row as TemplateRow
            const label = PLATFORM_LABELS[row.platform as keyof typeof PLATFORM_LABELS]
            return (
              <ElTag size="small" type="info">
                {label || `#${row.platform}`}
              </ElTag>
            )
          }
        }
      }
    },
    {
      field: 'adType',
      label: '广告类型',
      search: { hidden: true },
      table: {
        width: '90px',
        slots: {
          default: (data: any) => {
            const row: TemplateRow = data.row as TemplateRow
            const typeLabels: Record<string, string> = {
              APP: '应用',
              PIXEL: '像素',
              PRODUCT_CATALOG: '商品目录',
              PRODUCT_SET: '商品集',
              PERFORMANCE_MAX: 'P.Max',
              DISPLAY: '展示',
              MULTI_CHANNEL: '多渠道'
            }
            return (
              <span>
                {row.publishingAdType
                  ? typeLabels[row.publishingAdType] || row.publishingAdType
                  : '-'}
              </span>
            )
          }
        }
      }
    },
    {
      field: 'publishAdCount',
      label: '累计发布',
      search: { hidden: true },
      table: {
        width: '90px',
        align: 'center',
        slots: {
          default: (data: any) => {
            const row: TemplateRow = data.row as TemplateRow
            return (
              <ElTag size="small" type="success">
                {row.publishAdCount}
              </ElTag>
            )
          }
        }
      }
    },
    {
      field: 'lastPublishTime',
      label: '末次发布',
      search: { hidden: true },
      table: {
        width: '170px',
        slots: {
          default: (data: any) => {
            const row: TemplateRow = data.row as TemplateRow
            return <span>{row.lastPublishTime || '-'}</span>
          }
        }
      }
    },
    {
      field: 'creator',
      label: t('templateManage.creator'),
      search: { hidden: true },
      table: {
        width: '120px',
        slots: {
          default: (data: any) => {
            const row: TemplateRow = data.row as TemplateRow
            return <span>{row.creator || '-'}</span>
          }
        }
      }
    },
    {
      field: 'createdTime',
      label: '创建时间',
      search: { hidden: true },
      table: {
        width: '170px',
        slots: {
          default: (data: any) => {
            const row: TemplateRow = data.row as TemplateRow
            return <span>{row.createdTime || '-'}</span>
          }
        }
      }
    },
    {
      field: 'action',
      label: t('templateManage.action'),
      search: { hidden: true },
      form: { hidden: true },
      detail: { hidden: true },
      table: {
        fixed: 'right',
        width: '280px',
        slots: {
          default: (data: any) => {
            const row: TemplateRow = data.row as TemplateRow
            const menuContent = (
              <div style="padding:4px 0">
                <ElDropdownItem onClick={() => handlePublishWithTemplate(row)}>
                  {t('templateManage.publishWithTemplate')}
                </ElDropdownItem>
                <ElDropdownItem onClick={() => handleReproduction(row)}>
                  {t('templateManage.copyTemplate')}
                </ElDropdownItem>
                <ElDropdownItem onClick={() => handleViewDetail(row)}>
                  {t('templateManage.viewTemplate')}
                </ElDropdownItem>
                <ElDropdownItem divided onClick={() => handleRemove(row)}>
                  <span style="color:var(--el-color-danger)">{t('templateManage.goDelete')}</span>
                </ElDropdownItem>
              </div>
            )
            return (
              <div style="display:flex;align-items:center;gap:6px">
                <BaseButton
                  type="primary"
                  size="small"
                  onClick={() => handlePublishWithTemplate(row)}
                >
                  {t('templateManage.publishWithTemplate')}
                </BaseButton>
                <BaseButton size="small" onClick={() => handleEdit(row)}>
                  {t('common.edit')}
                </BaseButton>
                <ElDropdown trigger="click" placement="bottom-end">
                  {{
                    default: () => (
                      <ElButton size="small" style="padding:0 8px">
                        ···
                      </ElButton>
                    ),
                    dropdown: () => <ElDropdownMenu>{menuContent}</ElDropdownMenu>
                  }}
                </ElDropdown>
              </div>
            )
          }
        }
      }
    }
  ]
}

// ══════════════════════════════════════════
//  预计算列定义
// ══════════════════════════════════════════

const tableColumns = computed(() => {
  const cols = createColumns()
  const { allSchemas } = useCrudSchemas(cols)
  return allSchemas.tableColumns
})
</script>

<template>
  <ContentWrap>
    <!-- ═══════════ 页面标题 ═══════════ -->
    <div class="tm-page-header">
      <h2 class="tm-page-title">{{ t('templateManage.title') }}</h2>
    </div>

    <!-- ═══════════ 操作栏 ═══════════ -->
    <div class="tm-action-bar">
      <div class="tm-action-bar__left">
        <ElInput
          v-model="searchTemplateName"
          :placeholder="t('templateManage.templateNamePlaceholder')"
          clearable
          style="width: 240px"
          @keyup.enter="handleSearch"
          @clear="handleSearch"
        />
        <ElDatePicker
          v-model="searchTimeRange"
          type="daterange"
          start-placeholder="开始日期"
          end-placeholder="结束日期"
          value-format="YYYY-MM-DD"
          style="width: 260px"
        />
        <ElSelect
          v-model="searchPlatform"
          :placeholder="t('templateManage.mediaPlatform')"
          clearable
          style="width: 150px"
          @change="handleSearch"
        >
          <ElOption value="" :label="t('templateManage.allPlatforms')" />
          <ElOption
            v-for="opt in PLATFORM_OPTIONS"
            :key="opt.value"
            :label="opt.label"
            :value="Number(opt.value)"
          />
        </ElSelect>

        <ElInput
          v-model="searchCreator"
          :placeholder="t('templateManage.creator')"
          clearable
          style="width: 160px"
          @keyup.enter="handleSearch"
          @clear="handleSearch"
        />

        <ElInput
          v-model.number="searchPixelId"
          placeholder="像素 ID"
          clearable
          style="width: 140px"
          @keyup.enter="handleSearch"
          @clear="handleSearch"
        />
        <BaseButton type="primary" @click="handleSearch">{{
          t('templateManage.search')
        }}</BaseButton>
        <BaseButton @click="handleReset">{{ t('templateManage.reset') }}</BaseButton>
      </div>
      <div class="tm-action-bar__right">
        <BaseButton type="primary" @click="handleCreate">
          + {{ t('templateManage.createTemplate') }}
        </BaseButton>
      </div>
    </div>

    <!-- ═══════════ 模板列表 ═══════════ -->
    <Table
      v-model:pageSize="tableState.pageSize.value"
      v-model:currentPage="tableState.currentPage.value"
      :columns="tableColumns"
      :data="tableState.dataList.value"
      :loading="tableState.loading.value"
      :pagination="{ total: tableState.total.value }"
      @register="tableRegister"
    />

    <!-- ═══════════ 模板编辑器抽屉 ═══════════ -->
    <TemplateEditorDrawer
      v-if="editorVisible"
      :visible="editorVisible"
      :template-id="editingTemplateId"
      :initial-platform="initialPlatform"
      @update:visible="(v: boolean) => (editorVisible = v)"
      @on-success="handleEditorSuccess"
    />
  </ContentWrap>
</template>

<style scoped>
/* ═══════════ 页面标题 ═══════════ */
.tm-page-header {
  margin-bottom: 16px;
}

.tm-page-title {
  margin: 0;
  font-size: 18px;
  font-weight: 600;
  color: var(--el-text-color-primary);
}

/* ═══════════ 操作栏 ═══════════ */
.tm-action-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 12px;
  margin-bottom: 12px;
}

.tm-action-bar__left,
.tm-action-bar__right {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 10px;
}

/* ═══════════ 筛选行 ═══════════ */
.tm-filter-bar {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 10px;
  margin-bottom: 20px;
}

/* ═══════════ 模板信息 ═══════════ */
.tm-icon {
  display: flex;
  width: 40px;
  height: 40px;
  background: var(--el-color-primary-light-9);
  border-radius: 8px;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.tm-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
  min-width: 0;
}

.tm-name {
  overflow: hidden;
  font-size: 14px;
  font-weight: 600;
  color: var(--el-text-color-primary);
  text-overflow: ellipsis;
  white-space: nowrap;
}

.tm-sub {
  font-size: 12px;
  color: var(--el-text-color-secondary);
}
</style>
