import { User } from './user'

export interface Profile {
  username: string
  displayName: string
  image?: string
  bio?: string
  followersCount: number
  followingCount: number
  following: boolean
  photos?: Photo[]
}

// create a class, so that constructor can automatically set properties of the currently logged in user
// convert the object type from User to Profile
export class Profile implements Profile {
  constructor(user: User) {
    this.username = user.username
    this.displayName = user.displayName
    this.image = user.image
  }
}

export interface Photo {
  id: string
  url: string
  isMain: boolean
}

export interface UserActivity {
  id: string
  title: string
  category: string
  date: Date
}
