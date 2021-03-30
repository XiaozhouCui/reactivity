import { makeAutoObservable, reaction } from 'mobx'
import { ServerError } from './../models/serverError'

export default class CommonStore {
  // error is made an observable
  error: ServerError | null = null
  // store the token in common store
  token: string | null = window.localStorage.getItem('jwt')
  appLoaded = false

  constructor() {
    makeAutoObservable(this)
    // use mobx "reaction" function to persist login status
    // reaction function only runs if "this.token" changes
    reaction(
      // list what we want to react to: this.token
      () => this.token,
      token => {
        if (token) {
          window.localStorage.setItem('jwt', token)
        } else {
          window.localStorage.removeItem('jwt')
        }
      }
    )
  }

  setServerError = (error: ServerError) => {
    this.error = error
  }

  setToken = (token: string | null) => {
    this.token = token
  }

  setAppLoaded = () => {
    this.appLoaded = true
  }
}
