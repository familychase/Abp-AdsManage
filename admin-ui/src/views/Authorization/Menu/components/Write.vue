<script setup lang="tsx">
import { Form, FormSchema } from '@/components/Form'
import { useForm } from '@/hooks/web/useForm'
import { PropType, reactive, watch, ref, unref } from 'vue'
import { useValidator } from '@/hooks/web/useValidator'
import { useI18n } from '@/hooks/web/useI18n'
import { getMenuTreeForSelectApi } from '@/api/authorization/menu'
import { cloneDeep } from 'lodash-es'

const { t } = useI18n()

const { required } = useValidator()

const props = defineProps({
  currentRow: {
    type: Object as PropType<any>,
    default: () => null
  }
})

const permissionOptions = [
  { label: '查询列表', value: 'list' },
  { label: '新增', value: 'create' },
  { label: '编辑', value: 'update' },
  { label: '删除', value: 'delete' }
]

const formSchema = reactive<FormSchema[]>([
  {
    field: 'type',
    label: '菜单类型',
    component: 'RadioButton',
    value: 'DIRECTORY',
    colProps: {
      span: 24
    },
    componentProps: {
      options: [
        {
          label: '目录',
          value: 'DIRECTORY'
        },
        {
          label: '菜单',
          value: 'MENU'
        }
      ],
      on: {
        change: async (val: string) => {
          const formData = await getFormData()
          if (val === 'MENU') {
            setSchema([
              {
                field: 'component',
                path: 'componentProps.disabled',
                value: false
              }
            ])
            setValues({
              component: unref(cacheComponent),
              permissionCode: permissionOptions.map((opt) => opt.value)
            })
          } else {
            setSchema([
              {
                field: 'component',
                path: 'componentProps.disabled',
                value: true
              }
            ])

            if (formData.parentId === void 0) {
              setValues({
                component: '#',
                permissionCode: []
              })
            } else {
              setValues({
                component: '##',
                permissionCode: []
              })
            }
          }
        }
      }
    }
  },
  {
    field: 'parentId',
    label: '父级菜单',
    component: 'TreeSelect',
    componentProps: {
      nodeKey: 'id',
      props: {
        label: 'title',
        value: 'id',
        children: 'children'
      },
      highlightCurrent: true,
      defaultExpandAll: true,
      expandOnClickNode: false,
      checkStrictly: true,
      checkOnClickNode: true,
      clearable: true,
      on: {
        change: async (val: number) => {
          const formData = await getFormData()
          if (val && formData.type === 'DIRECTORY') {
            setValues({
              component: '##'
            })
          } else if (!val && formData.type === 'DIRECTORY') {
            setValues({
              component: '#'
            })
          } else if (formData.type === 'MENU') {
            setValues({
              component: unref(cacheComponent) ?? ''
            })
          }
        }
      }
    },
    optionApi: async () => {
      const res = await getMenuTreeForSelectApi('DIRECTORY')
      return res || []
    }
  },
  {
    field: 'meta.title',
    label: t('menu.menuName'),
    component: 'Input'
  },
  {
    field: 'component',
    label: '组件路径',
    component: 'Input',
    value: '#',
    componentProps: {
      disabled: true,
      placeholder: '#为顶级目录，##为子目录',
      on: {
        change: (val: string) => {
          cacheComponent.value = val
        }
      }
    }
  },
  {
    field: 'meta.nameEn',
    label: '菜单英文名',
    component: 'Input'
  },
  {
    field: 'name',
    label: t('menu.name'),
    component: 'Input'
  },
  {
    field: 'meta.icon',
    label: t('menu.icon'),
    component: 'IconPicker'
  },
  {
    field: 'path',
    label: t('menu.path'),
    component: 'Input'
  },
  {
    field: 'permissionCode',
    label: '权限标识',
    component: 'Select',
    componentProps: {
      allowCreate: true,
      filterable: true,
      clearable: true,
      multiple: true,
      placeholder: '请选择或输入权限标识',
      options: permissionOptions
    }
  },
  {
    field: 'sort',
    label: '排序',
    component: 'InputNumber',
    value: 0,
    componentProps: {
      min: 0,
      max: 9999,
      placeholder: '排序号'
    }
  },
  {
    field: 'meta.hidden',
    label: t('menu.hidden'),
    component: 'Switch'
  }
])

const rules = reactive({
  component: [required()],
  path: [required()],
  'meta.title': [required()]
})

const { formRegister, formMethods } = useForm()
const { setValues, getFormData, getElFormExpose, setSchema } = formMethods

const submit = async () => {
  const elForm = await getElFormExpose()
  const valid = await elForm?.validate().catch((err) => {
    console.log(err)
  })
  if (valid) {
    const formData = await getFormData()
    return formData
  }
}

const cacheComponent = ref('')

watch(
  () => props.currentRow,
  (value) => {
    if (!value) return
    const currentRow = cloneDeep(value)
    cacheComponent.value = currentRow.type === 'MENU' ? currentRow.component : ''
    if (currentRow.parentId === 0) {
      setSchema([
        {
          field: 'component',
          path: 'componentProps.disabled',
          value: true
        }
      ])
    } else {
      setSchema([
        {
          field: 'component',
          path: 'componentProps.disabled',
          value: false
        }
      ])
    }
    if (currentRow.type === 'MENU') {
      setSchema([
        {
          field: 'component',
          path: 'componentProps.disabled',
          value: false
        }
      ])
    } else {
      setSchema([
        {
          field: 'component',
          path: 'componentProps.disabled',
          value: true
        }
      ])
    }
    // 将后端返回的 permissionCode（"prefix:list,prefix:create"）转为 action 数组
    if (typeof currentRow.permissionCode === 'string') {
      const parts = currentRow.permissionCode.split(',').filter(Boolean)
      currentRow.permissionCode = parts.map((p: string) => {
        const segs = p.split(':')
        return segs[segs.length - 1] // 取最后一个冒号后的值
      })
    }
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
