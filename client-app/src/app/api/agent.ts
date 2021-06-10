import axios, { AxiosError, AxiosResponse } from 'axios'
import { toast } from 'react-toastify'
import { history } from '../..'
import { store } from '../stores/store'
import { Activity, ActivityFormValues } from '../models/activity'
import { User, UserFormValues } from '../models/user'
import { Photo, Profile } from '../models/profile'

const sleep = (delay: number) => {
  return new Promise((resolve) => {
    setTimeout(resolve, delay)
  })
}

axios.defaults.baseURL = 'http://localhost:5000/api'

axios.interceptors.request.use(config => {
  const token = store.commonStore.token
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

axios.interceptors.response.use(
  async (response) => {
    await sleep(500)
    return response
  },
  (error: AxiosError) => {
    const { data, status, config } = error.response!
    switch (status) {
      case 400:
        if (typeof data === 'string') {
          toast.error(data)
        }
        // case for invalid guid
        if (config.method === 'get' && data.errors.hasOwnProperty('id')) {
          history.push('/not-found')
        }
        if (data.errors) {
          const modalStateErrors = []
          for (const key in data.errors) {
            if (data.errors[key]) {
              modalStateErrors.push(data.errors[key])
            }
          }
          // throw back to the component as a flattened list of strings
          throw modalStateErrors.flat()
        }
        break
      case 401:
        toast.error('unauthorised')
        break
      case 404:
        // use the custom history object outside React components
        history.push('/not-found')
        break
      case 500:
        store.commonStore.setServerError(data)
        history.push('/server-error')
        break
    }
    return Promise.reject(error)
  }
)

// Adding TYPE SAFETY for response: what type of data will be returned from api
// use generic type <T> for responseBody, <T> can be <Activity>, <User>, <Profile>...
const responseBody = <T>(response: AxiosResponse<T>) => response.data

const requests = {
  get: <T>(url: string) => axios.get<T>(url).then(responseBody),
  post: <T>(url: string, body: {}) => axios.post<T>(url, body).then(responseBody),
  put: <T>(url: string, body: {}) => axios.put<T>(url, body).then(responseBody),
  del: <T>(url: string) => axios.delete<T>(url).then(responseBody),
}

const Activities = {
  // in list(), <T> is <Activity[]>
  list: () => requests.get<Activity[]>('/activities'),
  // in details(), <T> is <Activity>
  details: (id: string) => requests.get<Activity>(`/activities/${id}`),
  // in following methods, <T> is <void>, meaning not return anything from request
  create: (activity: ActivityFormValues) => requests.post<void>('/activities', activity),
  update: (activity: ActivityFormValues) =>
    requests.put<void>(`/activities/${activity.id}`, activity),
  delete: (id: string) => requests.del<void>(`/activities/${id}`),
  // attend activity: POST an empty object, and return viod
  attend: (id: string) => requests.post<void>(`/activities/${id}/attend`, {})
}

const Account = {
  current: () => requests.get<User>('/account'),
  login: (user: UserFormValues) => requests.post<User>('/account/login', user),
  register: (user: UserFormValues) =>
    requests.post<User>('/account/register', user),
}

// object to get user's profile
const Profiles = {
  // get method returns a promise containing Profile
  get: (username: string) => requests.get<Profile>(`/profiles/${username}`),
  // upload photo to API
  uploadPhoto: (file: Blob) => {
    let formData = new FormData()
    // 'File' needs to match the name of the property in API
    formData.append('File', file)
    // post request shall return a type of Photo from API
    return axios.post<Photo>('photos', formData, {
      headers: { 'Content-type': 'multipart/form-data' }
    })
  },
  setMainPhoto: (id: string) => requests.post(`/photos/${id}/setMain`, {}),
  deletePhoto: (id: string) => requests.del(`/photos/${id}`),
}

const agent = { Activities, Account, Profiles }

export default agent
