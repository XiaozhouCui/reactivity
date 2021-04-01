import { User } from "./user";

export interface Profile {
  username: string
  displayName: string
  image?: string
  bio?: string
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