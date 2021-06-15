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
        .withUrl('http://localhost:5000/chat?activityId=' + activityId, {
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
        runInAction(() => (this.comments = comments))
      })

      // when a new comment is received, update observable inside store
      // method name "ReceiveComment" needs to match exactly in SignalR (ChatHub.cs)
      this.hubConnection.on('ReceiveComment', (comment: ChatComment) => {
        // push new comment to store
        runInAction(() => this.comments.push(comment))
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
}
