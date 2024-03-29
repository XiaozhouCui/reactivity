import { observer } from 'mobx-react-lite'
import { useEffect } from 'react'
import { useParams } from 'react-router'
import { Grid } from 'semantic-ui-react'
import LoadingComponent from '../../app/layout/LoadingComponent'
import { useStore } from '../../app/stores/store'
import ProfileContent from './ProfileContent'
import ProfileHeader from './ProfileHeader'

const ProfilePage = () => {
  // useParams need generic type to return username as property
  const { username } = useParams<{ username: string }>()
  const { profileStore } = useStore()
  const { loadProfile, loadingProfile, profile, setActiveTab } = profileStore

  useEffect(() => {
    // load user profile after render
    loadProfile(username)
    // on disposal, reset active tab in profile store
    return () => {
      setActiveTab(0)
    }
  }, [loadProfile, username, setActiveTab])

  if (loadingProfile) return <LoadingComponent content='Loading profile...' />

  return (
    <Grid>
      <Grid.Column width={16}>
        {profile && (
          <>
            <ProfileHeader profile={profile} />
            <ProfileContent profile={profile} />
          </>
        )}
      </Grid.Column>
    </Grid>
  )
}

export default observer(ProfilePage)
