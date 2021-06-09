import { makeAutoObservable, runInAction } from 'mobx'
import agent from '../api/agent'
import { Profile } from '../models/profile'
import { store } from './store'

export default class ProfileStore {
  profile: Profile | null = null
  loadingProfile = false
  uploading = false

  constructor() {
    makeAutoObservable(this)
  }

  get isCurrentUser() {
    // check if there is user in userStore and profile in profileStore
    if (store.userStore.user && this.profile) {
      return store.userStore.user.username === this.profile.username
    }
    return false
  }

  loadProfile = async (username: string) => {
    this.loadingProfile = true
    try {
      // get profile from API
      const profile = await agent.Profiles.get(username)
      runInAction(() => {
        this.profile = profile
        this.loadingProfile = false
      })
    } catch (error) {
      console.log(error)
      // turn off loading indicator
      runInAction(() => (this.loadingProfile = false))
    }
  }

  uploadPhoto = async (file: Blob) => {
    this.uploading = true
    try {
      const response = await agent.Profiles.uploadPhoto(file)
      const photo = response.data
      runInAction(() => {
        // push photo if profile is not null
        if (this.profile) {
          this.profile.photos?.push(photo)
          if (photo.isMain && store.userStore.user) {
            store.userStore.setImage(photo.url)
            this.profile.image = photo.url
          }
        }
        this.uploading = false
      })
    } catch (error) {
      console.log(error)
      runInAction(() => (this.uploading = false))
    }
  }
}
