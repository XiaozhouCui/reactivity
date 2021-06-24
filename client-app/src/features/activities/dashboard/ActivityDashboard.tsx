import { observer } from 'mobx-react-lite'
import { useEffect, useState } from 'react'
import InfiniteScroll from 'react-infinite-scroller'
import { Grid, Button, Loader } from 'semantic-ui-react'
import LoadingComponent from '../../../app/layout/LoadingComponent'
import { PagingParams } from '../../../app/models/pagination'
import { useStore } from '../../../app/stores/store'
import ActitityFilters from './ActitityFilters'
import ActivityList from './ActivityList'

const ActivityDashboard = () => {
  const { activityStore } = useStore()
  const { loadActivities, activityRegistry, setPagingParams, pagination } = activityStore
  // local state to show loading of the next batch of activity
  const [loadingNext, setLoadingNext] = useState(false)

  // load next batch of activities
  function handleGetNext() {
    setLoadingNext(true)
    setPagingParams(new PagingParams(pagination!.currentPage + 1)) // cause axios computed property to be updated
    loadActivities().then(() => setLoadingNext(false))
  }

  useEffect(() => {
    if (activityRegistry.size <= 1) loadActivities();
  }, [activityRegistry.size, loadActivities])

  // don't load the full page if it is only loading next batch of activities
  if (activityStore.loadingInitial && !loadingNext) return <LoadingComponent content='Loading activities...' />


  return (
    <Grid>
      <Grid.Column width='10'>
        <InfiniteScroll
          pageStart={0}
          loadMore={handleGetNext}
          hasMore={!loadingNext && !!pagination && pagination.currentPage < pagination.totalPages}
          initialLoad={false}
        >
          <ActivityList />
        </InfiniteScroll>
        {/* <Button
          floated='right'
          content='More...'
          positive
          onClick={handleGetNext}
          loading={loadingNext}
          disabled={pagination?.totalPages === pagination?.currentPage}
        /> */}
      </Grid.Column>
      <Grid.Column width='6'>
        <ActitityFilters />
      </Grid.Column>
      <Grid.Column width={10}>
        <Loader active={loadingNext} />
      </Grid.Column>
    </Grid>
  )
}

export default observer(ActivityDashboard)
