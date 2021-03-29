import { makeAutoObservable } from 'mobx'
import { ServerError } from './../models/serverError'

export default class CommonStore {
  // error is made an observable
  error: ServerError | null = null
  // store the token in common store
  token: string | null = null
  appLoaded = false

  constructor() {
    makeAutoObservable(this)
  }

  setServerError = (error: ServerError) => {
    this.error = error
  }

  setToken = (token: string | null) => {
    if (token) window.localStorage.setItem('jwt', token)
    this.token = token
  }

  setAppLoaded = () => {
    this.appLoaded = true
  }
}
