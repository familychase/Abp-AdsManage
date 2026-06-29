<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ContentWrap } from '@/components/ContentWrap'
import { BaseButton } from '@/components/Button'
import {
  ElTable,
  ElTableColumn,
  ElPagination,
  ElInput,
  ElButton,
  ElMessage,
  ElMessageBox,
  ElDrawer,
  ElForm,
  ElFormItem,
  ElTag,
  ElSelect,
  ElOption,
  ElInputNumber,
  ElEmpty
} from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import {
  getDictSortListApi,
  createDictSortApi,
  updateDictSortApi,
  deleteDictSortApi,
  getDictItemTreeApi,
  createDictItemApi,
  updateDictItemApi,
  batchCreateSortItemApi,
  getDictItemQueryApi
} from '@/api/system/dict'
import type { DictSortDto, DictItemDto } from '@/api/system/dict/types'

// ══════════════════════════════════════════
//  搜索
// ══════════════════════════════════════════

const searchForm = reactive({
  itemName: '',
  itemCode: '',
  itemValue: ''
})

// ══════════════════════════════════════════
//  字典分类列表（父级）
// ══════════════════════════════════════════

interface SortRow extends DictSortDto {
  items?: DictItemDto[]
  itemsLoading?: boolean
  itemsLoaded?: boolean
}

const sortList = ref<SortRow[]>([])
const sortLoading = ref(false)
const sortTotal = ref(0)
const sortPage = ref(1)
const sortPageSize = ref(20)

const fetchSortList = async () => {
  sortLoading.value = true
  try {
    const res = await getDictSortListApi({
      page: sortPage.value,
      pageSize: sortPageSize.value,
      filter: undefined
    })
    const payload = res?.data ?? res
    const items: DictSortDto[] = payload?.items ?? []
    sortList.value = items.map((s) => ({
      ...s,
      items: [],
      itemsLoading: false,
      itemsLoaded: false
    }))
    sortTotal.value = payload?.totalCount ?? 0
  } catch {
    sortList.value = []
    sortTotal.value = 0
  } finally {
    sortLoading.value = false
  }
}

const handleSortSizeChange = (size: number) => {
  sortPageSize.value = size
  sortPage.value = 1
  fetchSortList()
}

const handleSortCurrentChange = (page: number) => {
  sortPage.value = page
  fetchSortList()
}

// ══════════════════════════════════════════
//  字典项展开加载
// ══════════════════════════════════════════

const expandedRows = ref<Set<number>>(new Set())

const handleExpand = async (row: SortRow, expandedRowsList: SortRow[]) => {
  const expandedIds = new Set<number>()
  for (const r of expandedRowsList) {
    if (r.id !== undefined) expandedIds.add(r.id)
  }
  expandedRows.value = expandedIds

  if (expandedIds.has(row.id!) && !row.itemsLoaded) {
    row.itemsLoading = true
    try {
      const res = await getDictItemTreeApi(row.id!)
      row.items = res?.data ?? res ?? []
      row.itemsLoaded = true
    } catch {
      row.items = []
    } finally {
      row.itemsLoading = false
    }
  }
}

// ══════════════════════════════════════════
//  搜索
// ══════════════════════════════════════════

const handleSearch = () => {
  sortPage.value = 1
  fetchSortList()
}

const handleReset = () => {
  searchForm.itemName = ''
  searchForm.itemCode = ''
  searchForm.itemValue = ''
  handleSearch()
}

// ══════════════════════════════════════════
//  新增/编辑弹窗（字典分类 or 字典项）
// ══════════════════════════════════════════

type DialogMode = 'createSort' | 'editSort' | 'createItem' | 'editItem' | 'batchCreateItem'

const dialogVisible = ref(false)
const dialogTitle = ref('')
const dialogMode = ref<DialogMode>('createSort')
const dialogLoading = ref(false)
const formRef = ref<FormInstance>()
const currentSortId = ref<number | null>(null)

const sortForm = reactive({
  platform: 'NONE' as string,
  dictSortType: 'NONE' as string,
  dictSortName: '',
  dictSortCode: '',
  remarks: ''
})

const itemForm = reactive({
  dictSortId: 0 as number,
  dictItemCode: '',
  dictItemName: '',
  dictItemNameEN: '',
  dictItemValue: '',
  remarks: '',
  ordinal: 0 as number,
  isProduction: true as boolean
})

const editingItemId = ref<number | null>(null)
const editingSortId = ref<number | null>(null)

/** 批量新增：每行一个表单对象 */
interface BatchItemForm {
  dictItemCode: string
  dictItemName: string
  dictItemNameEN: string
  dictItemValue: string
  remarks: string
  ordinal: number
  isProduction: boolean
}

const batchItemForms = ref<BatchItemForm[]>([])
const batchSortId = ref<number>(0)
/** 批量操作的目标父级ID：等于 batchSortId 时表示新增顶级字典项，否则为新增子级 */
const batchParentItemId = ref<number>(0)

const addBatchRow = () => {
  batchItemForms.value.push({
    dictItemCode: '',
    dictItemName: '',
    dictItemNameEN: '',
    dictItemValue: '',
    remarks: '',
    ordinal: 0,
    isProduction: true
  })
}

const removeBatchRow = (index: number) => {
  batchItemForms.value.splice(index, 1)
}

const sortFormRules: FormRules = {
  platform: [{ required: true, message: '请选择媒体平台', trigger: 'change' }],
  dictSortType: [{ required: true, message: '请选择类型', trigger: 'change' }],
  dictSortName: [{ required: true, message: '请输入字典名称', trigger: 'blur' }],
  dictSortCode: [{ required: true, message: '请输入字典编号', trigger: 'blur' }]
}

const itemFormRules: FormRules = {
  dictSortId: [{ required: true, message: '请选择字典分类', trigger: 'change' }],
  dictItemName: [{ required: true, message: '请输入中文名称', trigger: 'blur' }],
  dictItemValue: [{ required: true, message: '请输入字典值', trigger: 'blur' }]
}

/** 媒体平台选项 */
const platformOptions = [
  { label: 'NONE', value: 'NONE' },
  { label: 'META', value: 'META' }
]

/** 类型选项 */
const dictTypeOptions = [
  { label: 'NONE', value: 'NONE' },
  { label: 'SYSTEM', value: 'SYSTEM' },
  { label: 'MEDIA', value: 'MEDIA' }
]

/** 打开新增分类弹窗 */
const handleAddSort = () => {
  dialogMode.value = 'createSort'
  dialogTitle.value = '新增'
  editingSortId.value = null
  sortForm.platform = 'NONE'
  sortForm.dictSortType = 'NONE'
  sortForm.dictSortName = ''
  sortForm.dictSortCode = ''
  sortForm.remarks = ''
  dialogVisible.value = true
}

/** 打开编辑分类弹窗 */
const handleEditSort = (row: SortRow) => {
  dialogMode.value = 'editSort'
  dialogTitle.value = '编辑字典分类'
  editingSortId.value = row.id ?? null
  sortForm.platform = row.platform ?? 'NONE'
  sortForm.dictSortType = row.dictSortType ?? 'NONE'
  sortForm.dictSortName = row.dictSortName ?? ''
  sortForm.dictSortCode = row.dictSortCode ?? ''
  sortForm.remarks = row.remarks ?? ''
  dialogVisible.value = true
}

/** 打开新增字典项弹窗 */
const handleAddItem = (sortId: number) => {
  dialogMode.value = 'createItem'
  dialogTitle.value = '新增字典项'
  editingItemId.value = null
  itemForm.dictSortId = sortId
  itemForm.dictItemValue = ''
  itemForm.dictItemName = ''
  itemForm.dictItemNameEN = ''
  itemForm.dictItemCode = ''
  itemForm.remarks = ''
  itemForm.ordinal = 0
  itemForm.isProduction = true
  currentSortId.value = sortId
  dialogVisible.value = true
}

/** 从分类行直接打开批量操作（顶级字典项） */
const handleBatchAddItem = async (sortId: number) => {
  dialogMode.value = 'batchCreateItem'
  dialogTitle.value = '批量操作'
  batchSortId.value = sortId
  batchParentItemId.value = sortId // 顶级，parent 就是分类本身
  batchItemForms.value = []

  // 查询已有字典项并预填
  const sort = sortList.value.find((s) => s.id === sortId)
  if (sort?.dictSortCode) {
    try {
      const res = await getDictItemQueryApi(sort.dictSortCode)
      const items = res?.data ?? res ?? []
      if (Array.isArray(items)) {
        batchItemForms.value = items.map((item: DictItemDto) => ({
          dictItemCode: item.dictItemCode ?? '',
          dictItemName: item.dictItemName ?? '',
          dictItemNameEN: item.dictItemNameEN ?? '',
          dictItemValue: item.dictItemValue ?? '',
          remarks: item.remarks ?? '',
          ordinal: item.ordinal ?? 0,
          isProduction: item.isProduction ?? true
        }))
      }
    } catch {
      // 查询失败，给一个空行
    }
  }

  // 如果没有数据，给一个空行
  if (batchItemForms.value.length === 0) {
    addBatchRow()
  }
  dialogVisible.value = true
}

/** 从字典项行打开批量操作（新增子级） */
const handleBatchAddChildItem = (sortId: number, parentItemId: number) => {
  dialogMode.value = 'batchCreateItem'
  dialogTitle.value = '批量操作(子级)'
  batchSortId.value = sortId
  batchParentItemId.value = parentItemId
  batchItemForms.value = []

  // 从已加载的字典项树中查找父级已有子项并预填
  const sort = sortList.value.find((s) => s.id === sortId)
  if (sort?.items?.length) {
    // 递归查找目标父级
    const findItem = (list: DictItemDto[]): DictItemDto | undefined => {
      for (const item of list) {
        if (item.id === parentItemId) return item
        if (item.children?.length) {
          const found = findItem(item.children)
          if (found) return found
        }
      }
      return undefined
    }
    const parentItem = findItem(sort.items)
    const existingChildren = parentItem?.children ?? []
    if (existingChildren.length) {
      batchItemForms.value = existingChildren.map((item: DictItemDto) => ({
        dictItemCode: item.dictItemCode ?? '',
        dictItemName: item.dictItemName ?? '',
        dictItemNameEN: item.dictItemNameEN ?? '',
        dictItemValue: item.dictItemValue ?? '',
        remarks: item.remarks ?? '',
        ordinal: item.ordinal ?? 0,
        isProduction: item.isProduction ?? true
      }))
    }
  }

  if (batchItemForms.value.length === 0) {
    addBatchRow()
  }
  dialogVisible.value = true
}

/** 提交弹窗 */
const handleDialogSubmit = async () => {
  // 批量模式无需表单校验
  if (dialogMode.value !== 'batchCreateItem') {
    const valid = await formRef.value!.validate().catch(() => false)
    if (!valid) return
  }

  dialogLoading.value = true
  try {
    if (dialogMode.value === 'createSort') {
      await createDictSortApi(sortForm)
      ElMessage.success('新增字典分类成功')
    } else if (dialogMode.value === 'editSort') {
      await updateDictSortApi(editingSortId.value!, sortForm)
      ElMessage.success('编辑字典分类成功')
    } else if (dialogMode.value === 'createItem') {
      await createDictItemApi({ ...itemForm, parentId: currentSortId.value })
      ElMessage.success('新增字典项成功')
    } else if (dialogMode.value === 'editItem') {
      await updateDictItemApi(editingItemId.value!, itemForm)
      ElMessage.success('编辑字典项成功')
    } else if (dialogMode.value === 'batchCreateItem') {
      if (batchItemForms.value.length === 0) {
        ElMessage.warning('请至少添加一行')
        dialogLoading.value = false
        return
      }
      const sort = sortList.value.find((s) => s.id === batchSortId.value)
      if (!sort) {
        ElMessage.error('请选择所属分类')
        dialogLoading.value = false
        return
      }
      await batchCreateSortItemApi({
        platform: sort.platform ?? 'NONE',
        dictSortType: sort.dictSortType ?? 'NONE',
        dictSortCode: sort.dictSortCode ?? '',
        dictSortName: sort.dictSortName ?? '',
        remarks: sort.remarks ?? '',
        dictItems: batchItemForms.value.map((row) => ({
          dictSortId: batchSortId.value,
          parentId: batchParentItemId.value,
          dictItemCode: row.dictItemCode || null,
          dictItemName: row.dictItemName,
          dictItemNameEN: row.dictItemNameEN || null,
          dictItemValue: row.dictItemValue,
          remarks: row.remarks || null,
          ordinal: row.ordinal,
          isProduction: row.isProduction
        }))
      })
      ElMessage.success(`批量操作成功：${batchItemForms.value.length} 条`)
    }

    dialogVisible.value = false

    // 刷新对应数据
    if (dialogMode.value === 'createSort' || dialogMode.value === 'editSort') {
      await fetchSortList()
    } else {
      // 刷新对应分类的子项
      const targetSortId =
        dialogMode.value === 'batchCreateItem' ? batchSortId.value : currentSortId.value
      const sort = sortList.value.find((s) => s.id === targetSortId)
      if (sort) {
        sort.itemsLoaded = false
        sort.items = []
        // 重新加载
        sort.itemsLoading = true
        try {
          const res = await getDictItemTreeApi(targetSortId!)
          sort.items = res?.data ?? res ?? []
          sort.itemsLoaded = true
        } catch {
          sort.items = []
        } finally {
          sort.itemsLoading = false
        }
      }
    }
  } catch (err: any) {
    ElMessage.error(err?.message || '操作失败')
  } finally {
    dialogLoading.value = false
  }
}

/** 提交中禁止关闭，操作完成后由 submit 逻辑控制 */
const handleBeforeClose = (done: () => void) => {
  if (dialogLoading.value) {
    ElMessage.warning('正在提交，请等待完成')
    return
  }
  done()
}

/** 关闭弹窗 */
const handleDialogClose = () => {
  formRef.value?.resetFields()
}

// ══════════════════════════════════════════
//  删除
// ══════════════════════════════════════════

const handleDeleteSort = async (row: SortRow) => {
  try {
    await ElMessageBox.confirm(
      `确定删除字典分类 "${row.dictSortName}" 吗？此操作会同时删除其下所有字典项。`,
      '删除确认',
      { confirmButtonText: '确定', cancelButtonText: '取消', type: 'warning' }
    )
  } catch {
    return
  }
  try {
    await deleteDictSortApi(row.id!)
    ElMessage.success('删除成功')
    await fetchSortList()
  } catch (err: any) {
    ElMessage.error(err?.message || '删除失败')
  }
}

// ══════════════════════════════════════════
//  展开所有行
// ══════════════════════════════════════════

const expandingAll = ref(false)

const handleExpandAll = async () => {
  expandingAll.value = true
  const ids = new Set<number>()
  for (const sort of sortList.value) {
    if (sort.id !== undefined) {
      ids.add(sort.id)
      if (!sort.itemsLoaded) {
        sort.itemsLoading = true
        try {
          const res = await getDictItemTreeApi(sort.id)
          sort.items = res?.data ?? res ?? []
          sort.itemsLoaded = true
        } catch {
          sort.items = []
        } finally {
          sort.itemsLoading = false
        }
      }
    }
  }
  expandedRows.value = ids
  expandingAll.value = false
}

// ══════════════════════════════════════════
//  收起所有行
// ══════════════════════════════════════════

const handleCollapseAll = () => {
  expandedRows.value = new Set()
}

// ══════════════════════════════════════════
//  生命周期
// ══════════════════════════════════════════

onMounted(() => {
  fetchSortList()
})
</script>

<template>
  <ContentWrap title="字典设置">
    <!-- ═══════════ 搜索栏 ═══════════ -->
    <div class="dict-search-bar">
      <div class="dict-search-fields">
        <div class="dict-search-field">
          <label class="dict-search-label">字典项中文名称</label>
          <ElInput
            v-model="searchForm.itemName"
            placeholder="请输入字典项中文名称"
            clearable
            style="width: 200px"
            @keyup.enter="handleSearch"
            @clear="handleSearch"
          />
        </div>
        <div class="dict-search-field">
          <label class="dict-search-label">字典项编码</label>
          <ElInput
            v-model="searchForm.itemCode"
            placeholder="请输入字典项编码"
            clearable
            style="width: 200px"
            @keyup.enter="handleSearch"
            @clear="handleSearch"
          />
        </div>
        <div class="dict-search-field">
          <label class="dict-search-label">字典项值</label>
          <ElInput
            v-model="searchForm.itemValue"
            placeholder="请输入字典项值"
            clearable
            style="width: 200px"
            @keyup.enter="handleSearch"
            @clear="handleSearch"
          />
        </div>
        <ElButton type="primary" @click="handleSearch">搜索</ElButton>
        <ElButton @click="handleReset">重置</ElButton>
      </div>
    </div>

    <!-- ═══════════ 操作按钮栏 ═══════════ -->
    <div class="dict-action-bar">
      <div class="dict-action-left">
        <BaseButton type="primary" @click="handleAddSort">新增</BaseButton>
      </div>
      <div class="dict-action-right">
        <BaseButton
          :loading="expandingAll"
          :disabled="sortList.length === 0"
          @click="handleExpandAll"
        >
          展开全部
        </BaseButton>
        <BaseButton :disabled="expandedRows.size === 0" @click="handleCollapseAll">
          收起全部
        </BaseButton>
      </div>
    </div>

    <!-- ═══════════ 表格 ═══════════ -->
    <ElTable
      v-loading="sortLoading"
      :data="
        sortList.filter((s) => {
          // 如果没有任何搜索条件，显示所有
          if (!searchForm.itemName && !searchForm.itemCode && !searchForm.itemValue) return true
          // 如果有搜索条件但子项未加载，显示该分类（因为可能是目标分类）
          if (!s.itemsLoaded) return true
          // 过滤子项中有匹配的分类
          return (s.items ?? []).some((item) => {
            const matchName =
              !searchForm.itemName || (item.dictItemName ?? '').includes(searchForm.itemName)
            const matchCode =
              !searchForm.itemCode || (item.dictItemCode ?? '').includes(searchForm.itemCode)
            const matchValue =
              !searchForm.itemValue || (item.dictItemValue ?? '').includes(searchForm.itemValue)
            return matchName && matchCode && matchValue
          })
        })
      "
      border
      stripe
      row-key="id"
      :expand-row-keys="[...expandedRows].map(String)"
      style="width: 100%"
      @expand-change="(row: any, rows: any) => handleExpand(row, rows)"
    >
      <ElTableColumn type="expand">
        <template #default="{ row: sortRow }">
          <div v-loading="sortRow.itemsLoading" class="dict-items-subtable">
            <template v-if="sortRow.itemsLoaded && (!sortRow.items || sortRow.items.length === 0)">
              <ElEmpty description="暂无字典项">
                <BaseButton type="primary" size="small" @click="handleAddItem(sortRow.id!)">
                  新增字典项
                </BaseButton>
              </ElEmpty>
            </template>
            <template v-else-if="sortRow.itemsLoaded && sortRow.items?.length">
              <div class="dict-sub-header">
                <span class="dict-sub-title">字典项列表</span>
                <div class="dict-sub-header-actions">
                  <BaseButton type="primary" size="small" @click="handleAddItem(sortRow.id!)">
                    新增字典项
                  </BaseButton>
                  <BaseButton type="warning" size="small" @click="handleBatchAddItem(sortRow.id!)">
                    批量操作
                  </BaseButton>
                </div>
              </div>
              <ElTable
                :data="sortRow.items"
                border
                size="small"
                class="dict-inner-table"
                row-key="id"
                :tree-props="{ children: 'children' }"
                default-expand-all
              >
                <ElTableColumn
                  prop="dictItemCode"
                  label="编码"
                  min-width="140"
                  show-overflow-tooltip
                />
                <ElTableColumn
                  prop="dictItemName"
                  label="字典项中文名称"
                  min-width="160"
                  show-overflow-tooltip
                />
                <ElTableColumn
                  prop="dictItemNameEN"
                  label="字典项英文名称"
                  min-width="160"
                  show-overflow-tooltip
                />
                <ElTableColumn
                  prop="dictItemValue"
                  label="字典项值"
                  min-width="120"
                  show-overflow-tooltip
                />
                <ElTableColumn prop="remarks" label="备注" min-width="160" show-overflow-tooltip />
                <ElTableColumn prop="isProduction" label="环境" width="110">
                  <template #default="{ row: itemRow }">
                    <ElTag :type="itemRow.isProduction ? 'success' : 'info'" size="small">
                      {{ itemRow.isProduction ? '生产环境' : '非生产环境' }}
                    </ElTag>
                  </template>
                </ElTableColumn>
                <ElTableColumn label="操作" fixed="right" width="100">
                  <template #default="{ row: itemRow }">
                    <BaseButton
                      type="warning"
                      size="small"
                      link
                      @click="handleBatchAddChildItem(sortRow.id!, itemRow.id!)"
                    >
                      批量操作
                    </BaseButton>
                  </template>
                </ElTableColumn>
              </ElTable>
            </template>
          </div>
        </template>
      </ElTableColumn>
      <ElTableColumn
        prop="dictSortName"
        label="字典名称(编码)"
        min-width="200"
        show-overflow-tooltip
      >
        <template #default="{ row }">
          <span class="dict-sort-name">{{ row.dictSortName }}</span>
          <span class="dict-sort-code">({{ row.dictSortCode }})</span>
        </template>
      </ElTableColumn>
      <ElTableColumn prop="remarks" label="备注" min-width="200" show-overflow-tooltip />
      <ElTableColumn prop="dictSortType" label="字典类型" width="110" />
      <ElTableColumn prop="platform" label="媒体平台" width="110" />
      <ElTableColumn prop="creationTime" label="创建时间" width="180" />
      <ElTableColumn label="操作" fixed="right" width="200">
        <template #default="{ row }">
          <BaseButton type="primary" size="small" link @click="handleEditSort(row)">
            编辑
          </BaseButton>
          <BaseButton type="danger" size="small" link @click="handleDeleteSort(row)">
            删除
          </BaseButton>
          <BaseButton type="warning" size="small" link @click="handleBatchAddItem(row.id!)">
            批量操作
          </BaseButton>
        </template>
      </ElTableColumn>
    </ElTable>

    <!-- ═══════════ 分页 ═══════════ -->
    <div class="dict-pagination">
      <ElPagination
        v-model:current-page="sortPage"
        v-model:page-size="sortPageSize"
        :page-sizes="[10, 20, 50, 100]"
        :total="sortTotal"
        layout="total, sizes, prev, pager, next, jumper"
        background
        @size-change="handleSortSizeChange"
        @current-change="handleSortCurrentChange"
      />
    </div>

    <!-- ═══════════ 新增/编辑侧边栏 ═══════════ -->
    <ElDrawer
      v-model="dialogVisible"
      :title="dialogTitle"
      direction="rtl"
      size="66%"
      :before-close="handleBeforeClose"
      @close="handleDialogClose"
    >
      <template #header>
        <h3 class="dict-drawer-title">{{ dialogTitle }}</h3>
      </template>
      <!-- 字典分类表单 -->
      <ElForm
        v-if="dialogMode === 'createSort' || dialogMode === 'editSort'"
        ref="formRef"
        :model="sortForm"
        :rules="sortFormRules"
        label-width="0"
        class="dict-sort-form"
      >
        <!-- 媒体平台：按钮组选择器 -->
        <ElFormItem label="" prop="platform" class="dict-form-item-inline">
          <div class="dict-field-group">
            <span class="dict-field-label"><em class="required-star">*</em> 媒体平台</span>
            <div class="dict-btn-group">
              <button
                v-for="opt in platformOptions"
                :key="opt.value"
                type="button"
                class="dict-btn-option"
                :class="{ active: sortForm.platform === opt.value }"
                @click="sortForm.platform = opt.value"
              >
                {{ opt.label }}
              </button>
            </div>
          </div>
        </ElFormItem>

        <!-- 类型：按钮组选择器 -->
        <ElFormItem label="" prop="dictSortType" class="dict-form-item-inline">
          <div class="dict-field-group">
            <span class="dict-field-label"><em class="required-star">*</em> 类型</span>
            <div class="dict-btn-group">
              <button
                v-for="opt in dictTypeOptions"
                :key="opt.value"
                type="button"
                class="dict-btn-option"
                :class="{ active: sortForm.dictSortType === opt.value }"
                @click="sortForm.dictSortType = opt.value"
              >
                {{ opt.label }}
              </button>
            </div>
          </div>
        </ElFormItem>

        <!-- 中文名称 -->
        <ElFormItem label="" prop="dictSortName" class="dict-form-item-inline">
          <div class="dict-field-group">
            <span class="dict-field-label"><em class="required-star">*</em> 中文名称</span>
            <ElInput v-model="sortForm.dictSortName" placeholder="请输入中文名称" />
          </div>
        </ElFormItem>

        <!-- 编码 -->
        <ElFormItem label="" prop="dictSortCode" class="dict-form-item-inline">
          <div class="dict-field-group">
            <span class="dict-field-label"><em class="required-star">*</em> 编码</span>
            <ElInput v-model="sortForm.dictSortCode" placeholder="请输入编码" />
          </div>
        </ElFormItem>

        <!-- 备注 -->
        <ElFormItem label="" class="dict-form-item-inline">
          <div class="dict-field-group">
            <span class="dict-field-label">备注</span>
            <ElInput
              v-model="sortForm.remarks"
              type="textarea"
              :rows="3"
              placeholder="请输入备注"
            />
          </div>
        </ElFormItem>
      </ElForm>

      <!-- 字典项表单 -->
      <ElForm
        v-else-if="dialogMode === 'createItem' || dialogMode === 'editItem'"
        ref="formRef"
        :model="itemForm"
        :rules="itemFormRules"
        label-width="0"
        class="dict-sort-form"
      >
        <ElFormItem label="" prop="dictSortId" class="dict-form-item-inline">
          <div class="dict-field-group">
            <span class="dict-field-label"><em class="required-star">*</em> 所属分类</span>
            <ElSelect
              v-model="itemForm.dictSortId"
              style="width: 100%"
              :disabled="dialogMode === 'editItem'"
            >
              <ElOption
                v-for="sort in sortList"
                :key="sort.id"
                :label="`${sort.dictSortName} (${sort.dictSortCode})`"
                :value="sort.id ?? 0"
              />
            </ElSelect>
          </div>
        </ElFormItem>

        <ElFormItem label="" prop="dictItemName" class="dict-form-item-inline">
          <div class="dict-field-group">
            <span class="dict-field-label"><em class="required-star">*</em> 中文名称</span>
            <ElInput v-model="itemForm.dictItemName" placeholder="请输入中文名称" />
          </div>
        </ElFormItem>

        <ElFormItem label="" class="dict-form-item-inline">
          <div class="dict-field-group">
            <span class="dict-field-label">英文名称</span>
            <ElInput v-model="itemForm.dictItemNameEN" placeholder="请输入英文名称" />
          </div>
        </ElFormItem>

        <ElFormItem label="" prop="dictItemValue" class="dict-form-item-inline">
          <div class="dict-field-group">
            <span class="dict-field-label"><em class="required-star">*</em> 字典值</span>
            <ElInput v-model="itemForm.dictItemValue" placeholder="请输入字典值" />
          </div>
        </ElFormItem>

        <ElFormItem label="" class="dict-form-item-inline">
          <div class="dict-field-group">
            <span class="dict-field-label">编码</span>
            <ElInput v-model="itemForm.dictItemCode" placeholder="请输入编码" />
          </div>
        </ElFormItem>

        <ElFormItem label="" class="dict-form-item-inline">
          <div class="dict-field-group">
            <span class="dict-field-label">排序</span>
            <ElInputNumber v-model="itemForm.ordinal" :min="0" style="width: 100%" />
          </div>
        </ElFormItem>

        <ElFormItem label="" class="dict-form-item-inline">
          <div class="dict-field-group">
            <span class="dict-field-label">生产环境</span>
            <ElSelect v-model="itemForm.isProduction" style="width: 100%">
              <ElOption label="是" :value="true" />
              <ElOption label="否" :value="false" />
            </ElSelect>
          </div>
        </ElFormItem>

        <ElFormItem label="" class="dict-form-item-inline">
          <div class="dict-field-group">
            <span class="dict-field-label">备注</span>
            <ElInput
              v-model="itemForm.remarks"
              type="textarea"
              :rows="3"
              placeholder="请输入备注"
            />
          </div>
        </ElFormItem>
      </ElForm>

      <!-- 批量操作字典项 -->
      <div v-if="dialogMode === 'batchCreateItem'" class="dict-batch-form">
        <div class="dict-field-group" style="margin-bottom: 12px">
          <span class="dict-field-label"><em class="required-star">*</em> 所属分类</span>
          <ElSelect v-model="batchSortId" style="width: 100%">
            <ElOption
              v-for="sort in sortList"
              :key="sort.id"
              :label="`${sort.dictSortName} (${sort.dictSortCode})`"
              :value="sort.id ?? 0"
            />
          </ElSelect>
        </div>
        <div class="dict-batch-toolbar">
          <BaseButton type="primary" size="small" @click="addBatchRow">添加一行</BaseButton>
        </div>
        <ElTable :data="batchItemForms" border size="small" class="dict-batch-table">
          <ElTableColumn label="#" type="index" width="50" />
          <ElTableColumn label="编码" min-width="110">
            <template #default="{ row }">
              <ElInput v-model="row.dictItemCode" size="small" placeholder="编码" />
            </template>
          </ElTableColumn>
          <ElTableColumn label="中文名称" min-width="130">
            <template #default="{ row }">
              <ElInput v-model="row.dictItemName" size="small" placeholder="中文名称" />
            </template>
          </ElTableColumn>
          <ElTableColumn label="英文名称" min-width="130">
            <template #default="{ row }">
              <ElInput v-model="row.dictItemNameEN" size="small" placeholder="英文名称" />
            </template>
          </ElTableColumn>
          <ElTableColumn label="字典值" min-width="110">
            <template #default="{ row }">
              <ElInput v-model="row.dictItemValue" size="small" placeholder="字典值" />
            </template>
          </ElTableColumn>
          <ElTableColumn label="排序" width="80">
            <template #default="{ row }">
              <ElInputNumber
                v-model="row.ordinal"
                size="small"
                :min="0"
                controls-position="right"
              />
            </template>
          </ElTableColumn>
          <ElTableColumn label="生产" width="70">
            <template #default="{ row }">
              <ElSelect v-model="row.isProduction" size="small">
                <ElOption label="是" :value="true" />
                <ElOption label="否" :value="false" />
              </ElSelect>
            </template>
          </ElTableColumn>
          <ElTableColumn label="备注" min-width="120">
            <template #default="{ row }">
              <ElInput v-model="row.remarks" size="small" placeholder="备注" />
            </template>
          </ElTableColumn>
          <ElTableColumn label="操作" width="60" fixed="right">
            <template #default="{ $index }">
              <BaseButton type="danger" size="small" link @click="removeBatchRow($index)"
                >删除</BaseButton
              >
            </template>
          </ElTableColumn>
        </ElTable>
      </div>

      <template #footer>
        <ElButton @click="dialogVisible = false">取消</ElButton>
        <ElButton type="primary" :loading="dialogLoading" @click="handleDialogSubmit">
          确定
        </ElButton>
      </template>
    </ElDrawer>
  </ContentWrap>
</template>

<style scoped>
/* ═══════════ 搜索栏 ═══════════ */
.dict-search-bar {
  padding: 16px 20px;
  margin-bottom: 16px;
  background: var(--el-fill-color-lighter);
  border-radius: 8px;
}

.dict-search-fields {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
  align-items: flex-end;
}

.dict-search-field {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.dict-search-label {
  font-size: 12px;
  color: var(--el-text-color-secondary);
}

/* ═══════════ 操作按钮栏 ═══════════ */
.dict-action-bar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 16px;
  flex-wrap: wrap;
  gap: 12px;
}

.dict-action-left,
.dict-action-right {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
  align-items: center;
}

/* ═══════════ 字典分类名称列 ═══════════ */
.dict-sort-name {
  font-weight: 600;
  color: var(--el-text-color-primary);
}

.dict-sort-code {
  margin-left: 6px;
  font-size: 12px;
  color: var(--el-text-color-placeholder);
}

/* ═══════════ 子表 ═══════════ */
.dict-items-subtable {
  padding: 12px 24px 16px;
  background: var(--el-fill-color-lighter);
}

.dict-sub-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 10px;
}

.dict-sub-title {
  font-size: 13px;
  font-weight: 600;
  color: var(--el-text-color-secondary);
}

.dict-inner-table {
  background: var(--el-bg-color);
}

.dict-sub-header-actions {
  display: flex;
  gap: 8px;
}

.dict-empty-actions {
  display: flex;
  gap: 8px;
  justify-content: center;
}

/* ═══════════ 批量新增 ═══════════ */
.dict-batch-form {
  margin-top: 16px;
}

.dict-batch-toolbar {
  margin-bottom: 10px;
}

.dict-batch-table {
  width: 100%;
}

.dict-batch-table :deep(.el-table__body-wrapper) {
  overflow-x: auto;
}

/* ═══════════ 分页 ═══════════ */
.dict-pagination {
  display: flex;
  justify-content: flex-end;
  margin-top: 16px;
}

/* ═══════════ 新增/编辑弹窗 — 字典分类表单（按钮组选择器） ═══════════ */
.dict-sort-form {
  padding: 4px 0;
}

.dict-form-item-inline :deep(.el-form-item__error) {
  position: relative;
}

.dict-field-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.dict-field-label {
  font-size: 14px;
  color: var(--el-text-color-primary);
}

.required-star {
  margin-right: 2px;
  font-style: normal;
  color: var(--el-color-danger);
}

/* 按钮组选择器 */
.dict-btn-group {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

.dict-btn-option {
  padding: 6px 20px;
  font-size: 13px;
  line-height: 1.5;
  color: var(--el-text-color-regular);
  cursor: pointer;
  background: #fff;
  border: 1px solid var(--el-border-color);
  border-radius: 4px;
  outline: none;
  transition: all 0.2s ease;
  user-select: none;
}

.dict-btn-option:hover {
  color: var(--el-color-primary);
  border-color: var(--el-color-primary-light-3);
}

.dict-btn-option.active {
  color: #fff;
  background-color: var(--el-color-primary);
  border-color: var(--el-color-primary);
}
</style>
