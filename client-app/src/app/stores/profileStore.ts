import { makeAutoObservable, runInAction } from 'mobx'
import agent from '../api/agent'
import { Photo, Profile } from '../models/profile'
import { store } from './store'

export default class ProfileStore {
  profile: Profile | null = null
  loadingProfile = false
  uploading = false
  loading = false

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

  setMainPhoto = async (photo: Photo) => {
    this.loading = true
    try {
      await agent.Profiles.setMainPhoto(photo.id)
      store.userStore.setImage(photo.url)
      runInAction(() => {
        if (this.profile && this.profile.photos) {
          this.profile.photos.find((p) => p.isMain)!.isMain = false
          this.profile.photos.find((p) => p.id === photo.id)!.isMain = true
          this.profile.image = photo.url
          this.loading = false
        }
      })
    } catch (error) {
      console.log(error)
      runInAction(() => (this.loading = false))
    }
  }

  deletePhoto = async (photo: Photo) => {
    this.loading = true
    try {
      await agent.Profiles.deletePhoto(photo.id)
      runInAction(() => {
        if (this.profile) {
          // remove the photo from profile
          this.profile.photos = this.profile.photos?.filter(
            (p) => p.id !== photo.id
          )
          this.loading = false
        }
      })
    } catch (error) {
      console.log(error)
      runInAction(() => (this.loading = false))
    }
  }

  // Partial<Profile>: only update 2 of the properties in Profile type
  updateProfile = async (profile: Partial<Profile>) => {
    this.loading = true
    try {
      await agent.Profiles.updateProfile(profile)
      runInAction(() => {
        if (
          profile.displayName &&
          profile.displayName !== store.userStore.user?.displayName
        ) {
          store.userStore.setDisplayName(profile.displayName)
        }
        // overwrite the existing profile with the Partial profile passed in
        this.profile = { ...this.profile, ...(profile as Profile) } // "as Profile" to make TypeScript happy
        this.loading = false
      })
    } catch (error) {
      console.log(error)
      runInAction(() => (this.loading = false))
    }
  }
}
