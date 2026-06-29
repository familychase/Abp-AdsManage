<script setup lang="tsx">
import { Form, FormSchema } from '@/components/Form'
import { useForm } from '@/hooks/web/useForm'
import { PropType, reactive, watch, ref, unref, nextTick } from 'vue'
import { useValidator } from '@/hooks/web/useValidator'
import { useI18n } from '@/hooks/web/useI18n'
import { ElTree, ElCheckboxGroup, ElCheckbox } from 'element-plus'
import { getMenuTreeApi } from '@/api/authorization/menu'

const { t } = useI18n()

const { required } = useValidator()

const props = defineProps({
  currentRow: {
    type: Object as PropType<any>,
    default: () => null
  }
})

const treeRef = ref<typeof ElTree>()

const formSchema = ref<FormSchema[]>([
  {
    field: 'roleName',
    label: t('role.roleName'),
    component: 'Input'
  },
  {
    field: 'status',
    label: t('menu.status'),
    component: 'Select',
    componentProps: {
      options: [
        {
          label: t('userDemo.disable'),
          value: 0
        },
        {
          label: t('userDemo.enable'),
          value: 1
        }
      ]
    }
  },
  {
    field: 'remark',
    label: t('userDemo.remark'),
    component: 'Input',
    colProps: {
      span: 24
    },
    componentProps: {
      type: 'textarea',
      rows: 3
    }
  },
  {
    field: 'menu',
    label: t('role.menu'),
    colProps: {
      span: 24
    },
    formItemProps: {
      slots: {
        default: () => {
          return (
            <>
              <div class="flex w-full">
                <div class="flex-1">
                  <ElTree
                    ref={treeRef}
                    show-checkbox
                    node-key="id"
                    highlight-current
                    default-expand-all
                    expand-on-click-node={false}
                    data={treeData.value}
                    onNode-click={nodeClick}
                  >
                    {{
                      default: (data) => {
                        return <span>{data.data.meta.title}</span>
                      }
                    }}
                  </ElTree>
                </div>
                <div class="flex-1">
                  {unref(currentTreeData) && unref(currentTreeData)?.permissionList ? (
                    <ElCheckboxGroup v-model={unref(currentTreeData).meta.permission}>
                      {unref(currentTreeData)?.permissionList.map((v: any) => {
                        return <ElCheckbox label={v.value}>{v.label}</ElCheckbox>
                      })}
                    </ElCheckboxGroup>
                  ) : null}
                </div>
              </div>
            </>
          )
        }
      }
    }
  }
])

const currentTreeData = ref()
const nodeClick = (treeData: any) => {
  currentTreeData.value = treeData
}

const rules = reactive({
  roleName: [required()],
  role: [required()],
  status: [required()]
})

const { formRegister, formMethods } = useForm()
const { setValues, getFormData, getElFormExpose } = formMethods

// 将新 API 节点转换为带 meta 和 permissionList 的结构
const transformTreeNode = (node: any): any => {
  const permissions: { label: string; value: string }[] = []
  const actionLabels: Record<string, string> = {
    list: '查询列表',
    create: '新增',
    update: '编辑',
    delete: '删除'
  }
  if (node.permissionCode) {
    const codes = node.permissionCode.split(',').filter(Boolean)
    const seen = new Set<string>()
    for (const code of codes) {
      const action = code.split(':').pop() || ''
      if (action && !seen.has(action)) {
        seen.add(action)
        permissions.push({ label: actionLabels[action] || action, value: action })
      }
    }
  }
  return {
    id: node.id,
    name: node.name,
    meta: {
      title: node.name,
      permission: permissions.map((p) => p.value)
    },
    permissionList: permissions,
    children: (node.children || []).map(transformTreeNode)
  }
}

const treeData = ref<any[]>([])
const getMenuList = async () => {
  try {
    const res = await getMenuTreeApi()
    treeData.value = (res || []).map(transformTreeNode)
    if (!props.currentRow) return
    await nextTick()
    const menuIds: number[] = props.currentRow.menuIds || []
    for (const id of menuIds) {
      unref(treeRef)?.setChecked(id, true, false)
    }
  } catch {
    // ignore
  }
}
getMenuList()

const submit = async () => {
  const elForm = await getElFormExpose()
  const valid = await elForm?.validate().catch((err) => {
    console.log(err)
  })
  if (valid) {
    const formData = await getFormData()
    formData.menuIds = (unref(treeRef)?.getCheckedKeys() || []).filter((id: any) => Number(id) > 0)
    return formData
  }
}

watch(
  () => props.currentRow,
  (currentRow) => {
    if (!currentRow) return
    setValues(currentRow)
  },
  {
    deep: true,
    immediate: true
  }
)

defineExpose({
  submit
})
</script>

<template>
  <Form :rules="rules" @register="formRegister" :schema="formSchema" />
</template>
