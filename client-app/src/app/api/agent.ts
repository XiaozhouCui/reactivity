import { Activity } from '../models/activity'
import axios, { AxiosError, AxiosResponse } from 'axios'
import { toast } from 'react-toastify'
import { history } from '../..'
import { store } from '../stores/store'

const sleep = (delay: number) => {
  return new Promise((resolve) => {
    setTimeout(resolve, delay)
  })
}

axios.defaults.baseURL = 'http://localhost:5000/api'

axios.interceptors.response.use(
  async (response) => {
    await sleep(1000)
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
// use generic type <T> for responseBody
const responseBody = <T>(response: AxiosResponse<T>) => response.data

const requests = {
  get: <T>(url: string) => axios.get<T>(url).then(responseBody),
  post: <T>(url: string, body: {}) => axios.post<T>(url).then(responseBody),
  put: <T>(url: string, body: {}) => axios.put<T>(url).then(responseBody),
  del: <T>(url: string) => axios.delete(url).then<T>(responseBody),
}

const Activities = {
  // in list(), <T> is <Activity[]>
  list: () => requests.get<Activity[]>('/activities'),
  // in details(), <T> is <Activity>
  details: (id: string) => requests.get<Activity>(`/activities/${id}`),
  // in following methods, <T> is <void>, meaning not return anything from request
  create: (activity: Activity) => axios.post<void>('/activities', activity),
  update: (activity: Activity) =>
    axios.put<void>(`/activities/${activity.id}`, activity),
  delete: (id: string) => axios.delete<void>(`/activities/${id}`),
}

const agent = { Activities }

export default agent
