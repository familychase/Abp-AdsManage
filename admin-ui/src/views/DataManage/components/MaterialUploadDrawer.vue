<script setup lang="ts">
import { ref, watch } from 'vue'
import { Icon } from '@/components/Icon'
import { BaseButton } from '@/components/Button'
import { ElDrawer, ElUpload, ElProgress, ElMessage } from 'element-plus'
import type { UploadFile, UploadRawFile } from 'element-plus'
import { uploadMaterialApi } from '@/api/ads/material'

const props = defineProps<{
  visible: boolean
}>()

const emit = defineEmits<{
  'update:visible': [value: boolean]
  success: []
  preview: [file: UploadFile, url: string]
}>()

// ══════════════════════════════════════════
//  文件上传
// ══════════════════════════════════════════

const fileList = ref<UploadFile[]>([])
const uploading = ref(false)
const fileProgress = ref<Record<string, number>>({})
const fileErrors = ref<Record<string, string>>({})

const handleFileChange = (_file: UploadFile, fileList_: UploadFile[]) => {
  fileList.value = fileList_
}

const removeFile = (file: UploadFile) => {
  const idx = fileList.value.indexOf(file)
  if (idx > -1) fileList.value.splice(idx, 1)
}

// ══════════════════════════════════════════
//  视频第一帧截图
// ══════════════════════════════════════════

const captureVideoFrame = (videoFile: File): Promise<File | undefined> => {
  return new Promise((resolve) => {
    const video = document.createElement('video')
    const url = URL.createObjectURL(videoFile)

    video.preload = 'metadata'
    video.muted = true
    video.playsInline = true

    video.onloadeddata = () => {
      video.currentTime = 0.1
    }

    video.onseeked = () => {
      const canvas = document.createElement('canvas')
      canvas.width = video.videoWidth
      canvas.height = video.videoHeight
      const ctx = canvas.getContext('2d')
      if (ctx) {
        ctx.drawImage(video, 0, 0)
        canvas.toBlob(
          (blob) => {
            URL.revokeObjectURL(url)
            video.remove()
            if (blob) {
              const coverFile = new File([blob], `${videoFile.name}_cover.jpg`, {
                type: 'image/jpeg'
              })
              resolve(coverFile)
            } else {
              resolve(undefined)
            }
          },
          'image/jpeg',
          0.8
        )
      } else {
        URL.revokeObjectURL(url)
        video.remove()
        resolve(undefined)
      }
    }

    video.onerror = () => {
      URL.revokeObjectURL(url)
      video.remove()
      resolve(undefined)
    }

    video.src = url
  })
}

// ══════════════════════════════════════════
//  上传
// ══════════════════════════════════════════

const isVideoFile = (file: File) => file.type.startsWith('video/')

const handleSubmitUpload = async () => {
  const rawFiles = fileList.value.filter((f) => f.raw).map((f) => f.raw as UploadRawFile)
  if (rawFiles.length === 0) {
    ElMessage.warning('请先选择要上传的素材文件')
    return
  }

  uploading.value = true
  fileProgress.value = {}
  fileErrors.value = {}

  let successCount = 0
  let failCount = 0
  const totalCount = rawFiles.length

  for (let i = 0; i < rawFiles.length; i++) {
    const file = rawFiles[i]

    // 视频文件提取第一帧作为封面
    let coverFile: File | undefined
    if (isVideoFile(file)) {
      coverFile = await captureVideoFrame(file)
    }

    try {
      await uploadMaterialApi(file, {
        coverFile,
        onProgress: (percent) => {
          const uid = fileList.value[i]?.uid
          if (uid) fileProgress.value[uid] = Math.round(percent)
        }
      })
      successCount++
    } catch (err: any) {
      failCount++
      const uid = fileList.value[i]?.uid
      if (uid) {
        fileErrors.value[uid] = err?.message || '上传失败'
        fileProgress.value[uid] = -1 // 标记失败状态
      }
    }
  }

  if (failCount === 0) {
    ElMessage.success(`成功上传 ${successCount}/${totalCount} 个素材`)
    resetForm()
    emit('success')
  } else if (successCount > 0) {
    ElMessage.warning(`成功 ${successCount} 个，失败 ${failCount} 个（详见下方红色标记）`)
  } else {
    ElMessage.error(`${failCount} 个素材全部上传失败`)
  }

  uploading.value = false
}

const resetForm = () => {
  fileList.value = []
  fileProgress.value = {}
  fileErrors.value = {}
}

// ══════════════════════════════════════════
//  Drawer 开关
// ══════════════════════════════════════════

const drawerVisible = ref(false)

watch(
  () => props.visible,
  (val) => {
    drawerVisible.value = val
    if (val) resetForm()
  }
)

const handleClose = () => {
  drawerVisible.value = false
  emit('update:visible', false)
}

// ══════════════════════════════════════════
//  工具函数
// ══════════════════════════════════════════

const isImage = (file: UploadFile) => file.raw?.type?.startsWith('image/')
const isVideo = (file: UploadFile) => file.raw?.type?.startsWith('video/')

const getObjectUrl = (raw?: UploadRawFile) => (raw ? URL.createObjectURL(raw) : '')

const handlePreview = (file: UploadFile) => {
  const url = file.raw ? URL.createObjectURL(file.raw) : ''
  emit('preview', file, url)
}
</script>

<template>
  <ElDrawer
    v-model="drawerVisible"
    title="上传素材"
    direction="rtl"
    size="700px"
    :destroy-on-close="true"
    :before-close="handleClose"
  >
    <div class="mu-drawer">
      <!-- 拖拽上传区域 -->
      <ElUpload
        v-model:file-list="fileList"
        :auto-upload="false"
        :show-file-list="false"
        multiple
        drag
        accept="video/*,image/*"
        class="mu-upload"
        :on-change="handleFileChange"
        :on-remove="removeFile"
      >
        <div class="mu-upload__content">
          <Icon icon="ep:upload" :size="36" color="#909399" />
          <div class="mu-upload__text">将素材拖到此出，或<em>点击上传</em></div>
          <div class="mu-upload__tips">
            <Icon icon="ep:info-filled" :size="12" color="#67c23a" />
            <span>关闭VPN将提升上传速度</span>
          </div>
          <div class="mu-upload__formats">
            <div>图片格式：gif,png,jpg,jpeg</div>
            <div>视频格式：mp4,mov,mpeg,avi</div>
            <div>图片大小：20MB</div>
            <div>视频大小：600MB</div>
          </div>
        </div>
      </ElUpload>

      <!-- 预览网格 -->
      <div v-if="fileList.length > 0" class="mu-preview-grid">
        <div
          v-for="file in fileList"
          :key="'pv-' + file.uid"
          class="mu-preview-card"
          :class="{ 'is-error': fileErrors[file.uid] }"
          @click="handlePreview(file)"
        >
          <!-- 删除按钮（卡片上方） -->
          <button
            v-if="!uploading"
            class="mu-preview-card__delete"
            title="移除此素材"
            @click.stop="removeFile(file)"
          >
            <Icon icon="ep:close" :size="12" />
          </button>
          <!-- 上传进度条 -->
          <ElProgress
            v-if="uploading && fileProgress[file.uid] !== undefined"
            :percentage="fileProgress[file.uid] > 0 ? fileProgress[file.uid] : 0"
            :stroke-width="4"
            :show-text="false"
            :status="fileErrors[file.uid] ? 'exception' : undefined"
            class="mu-preview-progress"
          />
          <div class="mu-preview-thumb">
            <template v-if="isImage(file)">
              <img :src="getObjectUrl(file.raw)" class="mu-preview-img" />
            </template>
            <template v-else-if="isVideo(file)">
              <div class="mu-preview-video">
                <video
                  v-if="file.raw"
                  :src="getObjectUrl(file.raw)"
                  preload="metadata"
                  muted
                  class="mu-preview-img"
                ></video>
                <div class="mu-preview-video__overlay">
                  <Icon icon="ep:video-play" :size="24" color="#fff" />
                </div>
              </div>
            </template>
            <template v-else>
              <div class="mu-preview-fallback">
                <Icon icon="ep:document" :size="24" color="#909399" />
              </div>
            </template>
            <div class="mu-preview-name" :title="file.name">{{ file.name }}</div>
          </div>
          <!-- 错误信息 -->
          <div v-if="fileErrors[file.uid]" class="mu-preview-error">
            <Icon icon="ep:warning-filled" :size="10" />
            <span>{{ fileErrors[file.uid] }}</span>
          </div>
        </div>
      </div>
    </div>

    <template #footer>
      <BaseButton @click="handleClose">取消</BaseButton>
      <BaseButton type="primary" @click="resetForm()">清空</BaseButton>
      <BaseButton
        type="primary"
        :loading="uploading"
        :disabled="fileList.length === 0"
        @click="handleSubmitUpload"
      >
        开始上传（{{ fileList.length }}）
      </BaseButton>
    </template>
  </ElDrawer>
</template>

<style scoped>
.mu-drawer {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

/* 上传区域 */
.mu-upload {
  width: 100%;
}

.mu-upload :deep(.el-upload) {
  width: 100%;
}

.mu-upload :deep(.el-upload-dragger) {
  width: 100%;
  height: auto;
  padding: 40px 20px;
  background: var(--el-fill-color-light);
  border: 2px dashed var(--el-border-color);
  border-radius: 8px;
}

.mu-upload :deep(.el-upload-dragger:hover) {
  border-color: var(--el-color-primary);
}

.mu-upload__content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12px;
}

.mu-upload__text {
  font-size: 14px;
  color: var(--el-text-color-regular);
}

.mu-upload__text em {
  font-style: normal;
  color: var(--el-color-primary);
  cursor: pointer;
}

.mu-upload__tips {
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: 12px;
  color: var(--el-text-color-placeholder);
}

.mu-upload__formats {
  margin-top: 8px;
  font-size: 12px;
  line-height: 1.8;
  color: var(--el-text-color-placeholder);
  text-align: center;
}

/* 预览网格 */
.mu-preview-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(160px, 1fr));
  gap: 12px;
}

.mu-preview-card {
  position: relative;
  overflow: hidden;
  cursor: pointer;
  background: var(--el-fill-color);
  border-radius: 8px;
  transition: transform 0.15s ease;
  aspect-ratio: 16 / 10;
}

.mu-preview-card.is-error {
  box-shadow: 0 0 0 2px #f56c6c;
}

.mu-preview-card:hover {
  transform: scale(1.03);
}

.mu-preview-card__delete {
  position: absolute;
  top: 4px;
  right: 4px;
  z-index: 2;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 22px;
  height: 22px;
  padding: 0;
  color: #fff;
  cursor: pointer;
  background: rgb(0 0 0 / 50%);
  border: none;
  border-radius: 4px;
  opacity: 0;
  transition: opacity 0.15s ease;
}

.mu-preview-card:hover .mu-preview-card__delete {
  opacity: 1;
}

.mu-preview-card__delete:hover {
  background: var(--el-color-danger);
}

.mu-preview-progress {
  position: absolute;
  top: 0;
  left: 0;
  z-index: 2;
  width: 100%;
}

.mu-preview-progress :deep(.el-progress-bar__outer) {
  background: rgb(0 0 0 / 30%);
  border-radius: 0;
}

.mu-preview-progress :deep(.el-progress-bar__inner) {
  border-radius: 0;
}

.mu-preview-error {
  display: flex;
  align-items: flex-start;
  gap: 3px;
  padding: 3px 6px;
  font-size: 11px;
  line-height: 1.3;
  color: #fff;
  background: rgb(245 108 108 / 85%);
}

.mu-preview-error span {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.mu-preview-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.mu-preview-thumb {
  position: relative;
  width: 100%;
  aspect-ratio: 16 / 10;
  overflow: hidden;
  background: #303133;
}

.mu-preview-video {
  position: relative;
  width: 100%;
  height: 100%;
}

.mu-preview-video__overlay {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgb(0 0 0 / 25%);
}

.mu-preview-fallback {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 100%;
}

.mu-preview-name {
  position: absolute;
  right: 0;
  bottom: 0;
  left: 0;
  padding: 2px 6px;
  overflow: hidden;
  font-size: 12px;
  color: #fff;
  text-overflow: ellipsis;
  white-space: nowrap;
  background: linear-gradient(to top, rgb(0 0 0 / 60%), transparent);
}
</style>
