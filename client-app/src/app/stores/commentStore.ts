import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from '@microsoft/signalr'
import { makeAutoObservable, runInAction } from 'mobx'
import { ChatComment } from '../models/comment'
import { store } from './store'

// use SignalR to connect to server via web socket
export default class CommentStore {
  comments: ChatComment[] = []
  hubConnection: HubConnection | null = null

  constructor() {
    // comments and hubConnection are marked as observables
    makeAutoObservable(this)
  }

  // create a new hub connection
  createHubConnection = (activityId: string) => {
    // before making connection, check that we have a selected activity in activity store
    if (store.activityStore.selectedActivity) {
      this.hubConnection = new HubConnectionBuilder()
        .withUrl(process.env.REACT_APP_CHAT_URL + '?activityId=' + activityId, {
          accessTokenFactory: () => store.userStore.user?.token!,
        })
        .withAutomaticReconnect() // reconnect to chat hub if connection is lost
        .configureLogging(LogLevel.Information)
        .build()
      // start connection
      this.hubConnection
        .start() // .start() returns a promise
        .catch((error) =>
          console.log('Error establishing the connection: ', error)
        )
      // once connected, we will receive all comments under that activity ID
      // method name "LoadComments" needs to match exactly in SignalR (ChatHub.cs)
      this.hubConnection.on('LoadComments', (comments: ChatComment[]) => {
        // update observable inside our store
        runInAction(() => {
          // convert the ISOString to Date object
          comments.forEach((comment) => {
            // 'Z' at the end of ISO String means UTC time
            comment.createdAt = new Date(comment.createdAt + 'Z')
          })
          this.comments = comments
        })
      })

      // when a new comment is received, update observable inside store
      // method name "ReceiveComment" needs to match exactly in SignalR (ChatHub.cs)
      this.hubConnection.on('ReceiveComment', (comment: ChatComment) => {
        // push new comment to store
        runInAction(() => {
          comment.createdAt = new Date(comment.createdAt)
          // unshift: place the new comment at the start of the array, latest comment at the top
          this.comments.unshift(comment)
        })
      })
    }
  }

  // stop hub connection
  stopHubConnection = () => {
    this.hubConnection
      ?.stop() // .stop() returns a promise
      .catch((error) => console.log('Error stopping connection: ', error))
  }

  // cleanup: clear comments when user disconnects the activity
  clearComments = () => {
    this.comments = []
    this.stopHubConnection()
  }

  // add new comment
  addComment = async (values: any) => {
    values.activityId = store.activityStore.selectedActivity?.id
    try {
      // invoke the 'SendComment' method inside ChatHub.cs
      await this.hubConnection?.invoke('SendComment', values)
      // then on 'ReceiveComment', all connected users will get new comment back from server and push it to this.comments array
    } catch (error) {
      console.log(error)
    }
  }
}
