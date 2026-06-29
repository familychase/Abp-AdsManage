<script setup lang="ts">
import { ref, watch, computed, nextTick, onMounted } from 'vue'
import { useI18n } from '@/hooks/web/useI18n'

const { t } = useI18n()

// ══════════════════════════════════════════
//  类型定义
// ══════════════════════════════════════════

export interface TagOption {
  label: string
  value: string
}

export interface TagSegment {
  type: 'text' | 'tag'
  value: string // text 段是输入的文字；tag 段是编码（如 {creative}）
  label?: string // tag 段展示的名称（如 素材名称）
}

// ══════════════════════════════════════════
//  Props & Emits
// ══════════════════════════════════════════

const props = withDefaults(
  defineProps<{
    modelValue?: string
    options: TagOption[]
    placeholder?: string
    disabled?: boolean
  }>(),
  {
    modelValue: '',
    disabled: false
  }
)

const placeholder = computed(() => props.placeholder || t('tagInput.placeholder'))

const emit = defineEmits<{
  (e: 'update:modelValue', val: string): void
}>()

// ══════════════════════════════════════════
//  解析 / 序列化
// ══════════════════════════════════════════

/** 将字符串解析为 Segment[] */
function parse(value: string | undefined): TagSegment[] {
  if (!value) return [{ type: 'text', value: '' }]

  const tagPatterns = props.options.map((o) => escapeRegex(o.value))
  if (tagPatterns.length === 0) return [{ type: 'text', value: '' }]

  const regex = new RegExp(`(${tagPatterns.join('|')})`, 'g')
  const segments: TagSegment[] = []
  let lastIndex = 0
  let match: RegExpExecArray | null

  while ((match = regex.exec(value)) !== null) {
    if (match.index > lastIndex) {
      segments.push({ type: 'text', value: value.slice(lastIndex, match.index) })
    }
    const matchedValue = match[0]
    const option = props.options.find((o) => o.value === matchedValue)
    segments.push({ type: 'tag', value: matchedValue, label: option?.label ?? matchedValue })
    lastIndex = regex.lastIndex
  }

  if (lastIndex < value.length) {
    segments.push({ type: 'text', value: value.slice(lastIndex) })
  }

  if (segments.length === 0) {
    segments.push({ type: 'text', value: '' })
  }

  return segments
}

function escapeRegex(s: string) {
  return s.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')
}

/** 将 Segment[] 序列化为字符串 */
function serialize(segments: TagSegment[]): string {
  return segments.map((s) => s.value).join('')
}

// ══════════════════════════════════════════
//  内部状态
// ══════════════════════════════════════════

const segments = ref<TagSegment[]>(parse(props.modelValue))

// 外部 → 内部同步（modelValue 变化）
watch(
  () => props.modelValue,
  (val) => {
    const current = serialize(segments.value)
    if (current !== val) {
      segments.value = parse(val)
    }
  }
)

// options 变化时重新解析（字典异步加载场景）
watch(
  () => props.options,
  () => {
    segments.value = parse(props.modelValue)
  }
)

/** 内部变更 → 向外 emit */
function emitUpdate() {
  const str = serialize(segments.value)
  emit('update:modelValue', str)
}

// ══════════════════════════════════════════
//  Option → 标签分类（按变量名分组展示）
// ══════════════════════════════════════════

/** 已用标签值集合 */
const usedTagValues = computed(() => {
  const set = new Set<string>()
  for (const s of segments.value) {
    if (s.type === 'tag') set.add(s.value)
  }
  return set
})

/** 可用选项（排除已选） */
const availableOptions = computed(() =>
  props.options.filter((o) => !usedTagValues.value.has(o.value))
)

// ══════════════════════════════════════════
//  操作
// ══════════════════════════════════════════

/** 在下拉中选择 → 在当前光标位置后插入标签 */
function insertTag(option: TagOption) {
  const idx = activeTextIndex.value
  const textSeg = segments.value[idx]
  if (!textSeg || textSeg.type !== 'text') return

  const textBefore = textSeg.value.slice(0, cursorPosition.value)
  const textAfter = textSeg.value.slice(cursorPosition.value)

  const newSegments: TagSegment[] = []
  if (textBefore) newSegments.push({ type: 'text', value: textBefore })
  newSegments.push({ type: 'tag', value: option.value, label: option.label })
  // 始终追加一个空 text 段在后面（确保标签后有可聚焦的输入位置）
  if (textAfter) {
    newSegments.push({ type: 'text', value: textAfter })
  } else {
    newSegments.push({ type: 'text', value: '' })
  }

  segments.value.splice(idx, 1, ...newSegments)
  emitUpdate()

  // 聚焦到标签后面的 text 段
  const focusIdx = idx + (textBefore ? 2 : 1)
  // 延迟聚焦，等 DOM 更新完成
  setTimeout(() => {
    const el = textInputRefs.value[focusIdx]
    if (el) {
      el.focus()
      activeTextIndex.value = focusIdx
      cursorPosition.value = el.value.length
    }
  }, 50)
}

/** 删除标签 → 合并前后 text */
function removeTag(idx: number) {
  const before = segments.value[idx - 1]
  const after = segments.value[idx + 1]

  if (before?.type === 'text' && after?.type === 'text') {
    before.value = before.value + after.value
    segments.value.splice(idx, 2)
  } else {
    // 没有相邻 text，移除 tag 和可能的 after
    segments.value.splice(idx, 2)
    // 确保至少有一个 text 段
    const hasText = segments.value.some((s) => s.type === 'text')
    if (!hasText) {
      segments.value.splice(idx, 0, { type: 'text', value: '' })
    }
  }

  // 确保首尾有 text 段
  if (segments.value.length > 0 && segments.value[0].type !== 'text') {
    segments.value.unshift({ type: 'text', value: '' })
  }
  if (segments.value.length > 0 && segments.value[segments.value.length - 1].type !== 'text') {
    segments.value.push({ type: 'text', value: '' })
  }
  if (segments.value.length === 0) {
    segments.value.push({ type: 'text', value: '' })
  }

  emitUpdate()
  nextTick(() => focusTextInput(Math.max(0, idx - 1)))
}

/** 更新 text 段的值 */
function updateText(idx: number, value: string) {
  if (segments.value[idx] && segments.value[idx].type === 'text') {
    segments.value[idx].value = value
    emitUpdate()
  }
}

// ══════════════════════════════════════════
//  焦点 & 光标管理
// ══════════════════════════════════════════

const textInputRefs = ref<Record<number, HTMLInputElement>>({})
const activeTextIndex = ref(0)
const cursorPosition = ref(0)

function setTextRef(idx: number, el: HTMLInputElement | null) {
  if (el) {
    textInputRefs.value[idx] = el
  }
}

function focusTextInput(idx: number) {
  activeTextIndex.value = idx
  cursorPosition.value = segments.value[idx]?.value?.length ?? 0
  nextTick(() => {
    textInputRefs.value[idx]?.focus()
    textInputRefs.value[idx]?.setSelectionRange(cursorPosition.value, cursorPosition.value)
  })
}

function onTextFocus(idx: number) {
  activeTextIndex.value = idx
}

function onTextInput(idx: number, e: Event) {
  const target = e.target as HTMLInputElement
  updateText(idx, target.value)
  cursorPosition.value = target.selectionStart ?? target.value.length
}

function onTextKeydown(idx: number, e: KeyboardEvent) {
  const input = e.target as HTMLInputElement

  // Backspace 到段开头
  if (e.key === 'Backspace' && input.selectionStart === 0 && input.selectionEnd === 0) {
    const prev = segments.value[idx - 1]
    // 前一段是标签 → 删除该标签，合并前后 text
    if (prev?.type === 'tag') {
      e.preventDefault()
      removeTag(idx - 1)
      nextTick(() => {
        // 光标放到合并后的位置（原来前一个 text 段的末尾）
        const el = textInputRefs.value[idx - 1]
        el?.focus()
      })
      return
    }
    // 前一段是 text → 合并
    if (prev?.type === 'text') {
      e.preventDefault()
      const removedValue = segments.value[idx].value
      prev.value = prev.value + removedValue
      segments.value.splice(idx, 1)
      emitUpdate()
      nextTick(() => {
        const el = textInputRefs.value[idx - 1]
        if (el) {
          el.focus()
          el.setSelectionRange(
            prev.value.length - removedValue.length,
            prev.value.length - removedValue.length
          )
        }
      })
    }
  }

  // Delete 到段末尾
  if (e.key === 'Delete' && input.selectionStart === input.value.length) {
    const next = segments.value[idx + 1]
    // 后一段是标签 → 删除该标签，合并前后 text
    if (next?.type === 'tag') {
      e.preventDefault()
      removeTag(idx + 1)
      // 光标保持在当前 text 段末尾
      nextTick(() => {
        const el = textInputRefs.value[idx]
        el?.focus()
        el?.setSelectionRange(el.value.length, el.value.length)
      })
      return
    }
    // 后一段是 text → 合并
    if (next?.type === 'text') {
      e.preventDefault()
      segments.value[idx].value = segments.value[idx].value + next.value
      segments.value.splice(idx + 1, 1)
      emitUpdate()
    }
  }
  // 上下左右箭头键后更新 cursorPosition
  if (['ArrowLeft', 'ArrowRight', 'ArrowUp', 'ArrowDown', 'Home', 'End'].includes(e.key)) {
    nextTick(() => {
      cursorPosition.value = input.selectionStart ?? input.value.length
    })
  }
}

function onTextClick(idx: number, e: MouseEvent) {
  activeTextIndex.value = idx
  nextTick(() => {
    const input = e.target as HTMLInputElement
    cursorPosition.value = input.selectionStart ?? input.value.length
  })
}

// ══════════════════════════════════════════
//  键盘导航：在 text 段之间切换
// ══════════════════════════════════════════

function onContainerClick(_e: MouseEvent) {
  // 点击容器空白区域 → 聚焦最后一个 text
  focusLastText()
}

function focusLastText() {
  const lastTextIdx = segments.value.length - 1
  if (lastTextIdx >= 0 && segments.value[lastTextIdx].type === 'text') {
    focusTextInput(lastTextIdx)
  }
}

/** 点击标签 → 聚焦相邻 text（方便继续输入） */
function onTagClick(idx: number) {
  const after = segments.value[idx + 1]
  if (after?.type === 'text') {
    focusTextInput(idx + 1)
  } else {
    const before = segments.value[idx - 1]
    if (before?.type === 'text') {
      focusTextInput(idx - 1)
    }
  }
}

// ══════════════════════════════════════════
//  挂载自动聚焦
// ══════════════════════════════════════════

onMounted(() => {
  if (!props.disabled && segments.value.length > 0) {
    const first = segments.value[0]
    if (first?.type === 'text') {
      nextTick(() => {
        textInputRefs.value[0]?.focus()
      })
    }
  }
})

// ══════════════════════════════════════════
//  下拉状态
// ══════════════════════════════════════════

const dropdownVisible = ref(false)

function toggleDropdown() {
  dropdownVisible.value = !dropdownVisible.value
}

function closeDropdown() {
  dropdownVisible.value = false
}

function onDropdownSelect(option: TagOption) {
  insertTag(option)
  dropdownVisible.value = false
}
</script>

<template>
  <div
    class="tag-input"
    :class="{ 'is-disabled': disabled, 'is-focused': dropdownVisible }"
    @click="onContainerClick"
  >
    <!-- ──── 内容区 ──── -->
    <div class="tag-input__inner">
      <template v-for="(seg, idx) in segments" :key="idx">
        <!-- 文本段 -->
        <input
          v-if="seg.type === 'text'"
          :ref="(el: any) => setTextRef(idx, el as HTMLInputElement)"
          class="tag-input__text"
          :class="{ 'is-empty': !seg.value }"
          :value="seg.value"
          :size="seg.value ? seg.value.length + 1 : undefined"
          :autofocus="idx === 0 && seg.type === 'text'"
          :placeholder="idx === 0 && segments.length === 1 ? placeholder : ''"
          :disabled="disabled"
          @input="onTextInput(idx, $event)"
          @focus="onTextFocus(idx)"
          @click="onTextClick(idx, $event)"
          @keydown="onTextKeydown(idx, $event)"
        />

        <!-- 标签段 -->
        <span v-else class="tag-input__tag" @click="onTagClick(idx)">
          <span class="tag-input__tag-label">{{ seg.label }}</span>
          <button
            v-if="!disabled"
            class="tag-input__tag-close"
            @click.stop="removeTag(idx)"
            type="button"
          >
            ×
          </button>
        </span>
      </template>
    </div>

    <!-- ──── 下拉触发器 ──── -->
    <div class="tag-input__dropdown-wrapper" v-if="!disabled">
      <button
        class="tag-input__dropdown-btn"
        :class="{ 'is-active': dropdownVisible }"
        @click.stop="toggleDropdown"
        type="button"
        :title="t('tagInput.insertVariable')"
      >
        <svg
          width="12"
          height="12"
          viewBox="0 0 24 24"
          fill="none"
          class="tag-input__dropdown-icon"
        >
          <path d="M6 9l6 6 6-6" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
        </svg>
      </button>

      <!-- 下拉菜单 -->
      <Transition name="tag-dropdown-fade">
        <div v-if="dropdownVisible" class="tag-input__dropdown">
          <div class="tag-input__dropdown-header">{{ t('tagInput.selectVariable') }}</div>
          <ul class="tag-input__dropdown-list">
            <li
              v-for="opt in availableOptions"
              :key="opt.value"
              class="tag-input__dropdown-item"
              @click="onDropdownSelect(opt)"
            >
              <span class="tag-input__dropdown-item-label">{{ opt.label }}</span>
              <code class="tag-input__dropdown-item-code">{{ opt.value }}</code>
            </li>
          </ul>
          <div v-if="availableOptions.length === 0" class="tag-input__dropdown-empty">
            {{ t('tagInput.allUsed') }}
          </div>
        </div>
      </Transition>

      <!-- 遮罩（点击外部关闭） -->
      <div v-if="dropdownVisible" class="tag-input__overlay" @click="closeDropdown"></div>
    </div>
  </div>
</template>

<style scoped>
/* ══════════════════════════════════════════
   TagInput — 标签+文本混合输入
   ══════════════════════════════════════════ */

.tag-input {
  display: flex;
  width: 100%;
  min-height: 32px;
  padding: 2px 4px;
  cursor: text;
  background: var(--el-fill-color-blank, #fff);
  border: 1px solid var(--el-border-color);
  border-radius: var(--el-border-radius-base, 4px);
  transition: border-color 0.2s;
  align-items: stretch;
}

.tag-input:hover {
  border-color: var(--el-border-color-hover);
}

.tag-input.is-focused {
  border-color: var(--el-color-primary);
  box-shadow: 0 0 0 1px var(--el-color-primary-light-7);
}

.tag-input.is-disabled {
  cursor: not-allowed;
  background: var(--el-disabled-bg-color, #f5f7fa);
}

/* ── 内容区 ── */
.tag-input__inner {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 0;
  flex: 1;
  min-width: 0;
  padding: 2px 4px;
}

/* ── 文本输入 ── */
.tag-input__text {
  width: auto;
  height: 24px;
  min-width: 0;
  padding: 0;
  font-family: inherit;
  font-size: 13px;
  line-height: 24px;
  color: var(--el-text-color-regular);
  background: transparent;
  border: none;
  outline: none;
  flex: 0 0 auto;
}

.tag-input__text.is-empty {
  width: 0;
  min-width: 0;
  overflow: hidden;
  flex: 0 0 0;
}

/* focus 时空 input 显示光标 */
.tag-input__text.is-empty:focus {
  width: 1ch;
  overflow: visible;
}

.tag-input__text::placeholder {
  color: var(--el-text-color-placeholder);
}

.tag-input__text:disabled {
  color: var(--el-text-color-disabled);
}

/* ── 标签 ── */
.tag-input__tag {
  display: inline-flex;
  height: 22px;
  padding: 0 3px;
  font-size: 12px;
  white-space: nowrap;
  cursor: pointer;
  background: var(--el-color-primary-light-9);
  border: 1px solid var(--el-color-primary-light-6);
  border-radius: 4px;
  transition: background 0.15s;
  user-select: none;
  align-items: center;
  gap: 2px;
}

.tag-input__tag:hover {
  background: var(--el-color-primary-light-8);
}

.tag-input__tag-label {
  font-weight: 500;
  color: var(--el-color-primary);
}

.tag-input__tag-close {
  display: inline-flex;
  width: 14px;
  height: 14px;
  padding: 0;
  font-size: 12px;
  line-height: 1;
  color: var(--el-color-primary);
  cursor: pointer;
  background: transparent;
  border: none;
  border-radius: 50%;
  transition: background 0.15s;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.tag-input__tag-close:hover {
  color: #fff;
  background: var(--el-color-primary);
}

/* ── 下拉按钮 ── */
.tag-input__dropdown-wrapper {
  position: relative;
  display: flex;
  align-items: center;
  flex-shrink: 0;
}

.tag-input__dropdown-btn {
  display: flex;
  width: 22px;
  height: 22px;
  padding: 0;
  color: var(--el-text-color-secondary);
  cursor: pointer;
  background: transparent;
  border: none;
  border-radius: 4px;
  transition: all 0.15s;
  align-items: center;
  justify-content: center;
}

.tag-input__dropdown-btn:hover {
  color: var(--el-color-primary);
  background: var(--el-fill-color-light);
}

.tag-input__dropdown-btn.is-active {
  color: var(--el-color-primary);
  background: var(--el-color-primary-light-9);
}

.tag-input__dropdown-btn.is-active .tag-input__dropdown-icon {
  transform: rotate(180deg);
}

.tag-input__dropdown-icon {
  transition: transform 0.2s;
}

/* ── 下拉菜单 ── */
.tag-input__dropdown {
  position: absolute;
  top: calc(100% + 4px);
  right: 0;
  z-index: 1000;
  max-height: 260px;
  min-width: 180px;
  overflow-y: auto;
  background: var(--el-bg-color-overlay, #fff);
  border: 1px solid var(--el-border-color-light);
  border-radius: 6px;
  box-shadow: 0 4px 16px rgb(0 0 0 / 10%);
}

.tag-input__dropdown-header {
  padding: 6px 10px;
  font-size: 11px;
  color: var(--el-text-color-secondary);
  border-bottom: 1px solid var(--el-border-color-lighter);
}

.tag-input__dropdown-list {
  padding: 4px 0;
  margin: 0;
  list-style: none;
}

.tag-input__dropdown-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 6px 10px;
  font-size: 13px;
  cursor: pointer;
  transition: background 0.1s;
}

.tag-input__dropdown-item:hover {
  background: var(--el-fill-color-light);
}

.tag-input__dropdown-item-label {
  color: var(--el-text-color-regular);
}

.tag-input__dropdown-item-code {
  padding: 1px 5px;
  font-size: 11px;
  color: var(--el-text-color-secondary);
  background: var(--el-fill-color);
  border-radius: 3px;
}

.tag-input__dropdown-empty {
  padding: 12px 10px;
  font-size: 12px;
  color: var(--el-text-color-placeholder);
  text-align: center;
}

/* ── 遮罩 ── */
.tag-input__overlay {
  position: fixed;
  inset: 0;
  z-index: 999;
}

/* ── 过渡动画 ── */
.tag-dropdown-fade-enter-active,
.tag-dropdown-fade-leave-active {
  transition:
    opacity 0.15s ease,
    transform 0.15s ease;
}

.tag-dropdown-fade-enter-from,
.tag-dropdown-fade-leave-to {
  opacity: 0;
  transform: translateY(-4px);
}
</style>
