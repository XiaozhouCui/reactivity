import { observer } from 'mobx-react-lite'
import React from 'react'
import { Link } from 'react-router-dom'
import { Card, Icon, Image } from 'semantic-ui-react'
import { Profile } from '../../app/models/profile'

interface Props {
  profile: Profile
}

const ProfileCard = ({ profile }: Props) => {
  // truncate: hide the overflow text to keep card size small
  function truncate(str: string | undefined) {
    if (str) {
      return str.length > 40 ? str.substring(0, 37) + '...' : str
    }
  }
  return (
    <Card as={Link} to={`/profiles/${profile.username}`}>
      <Image src={profile.image || '/assets/user.png'} />
      <Card.Content>
        <Card.Header>{profile.displayName}</Card.Header>
        <Card.Description>{truncate(profile.bio)}</Card.Description>
      </Card.Content>
      <Card.Content extra>
        <Icon name='user' />
        {profile.followersCount} followers
      </Card.Content>
    </Card>
  )
}

export default observer(ProfileCard)
