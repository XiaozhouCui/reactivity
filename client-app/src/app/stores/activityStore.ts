import { makeObservable, observable } from "mobx"

export default class ActivityStore {
    title = 'Hello from MobX!'
    constructor() {
        // makeObservable will use "this" class
        makeObservable(this, {
            title: observable
        })
    }
}