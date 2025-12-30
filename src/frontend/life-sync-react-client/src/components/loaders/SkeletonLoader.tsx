export const SkeletonLoader = () => {
  const skeletonClass =
    'animate-pulse rounded bg-muted motion-reduce:animate-none';

  return (
    <div
      className="grid min-h-dvh grid-cols-[minmax(0,1fr)] grid-rows-[auto_1fr_auto] bg-background"
      aria-label="Loading content"
    >
      <div className="mb-4 flex h-12 w-full items-center justify-between px-8 pt-4">
        <div className="flex items-center gap-4">
          <div className={`h-12 w-12 rounded-full ${skeletonClass}`} />
          <div className={`h-6 w-[120px] rounded ${skeletonClass}`} />
        </div>
        <div className="flex items-center gap-2">
          <div className={`h-6 w-6 rounded-full ${skeletonClass}`} />
          <div className={`h-6 w-6 rounded-full ${skeletonClass}`} />
        </div>
      </div>

      <div className="m-4 flex items-center justify-center">
        <div className={`h-[400px] w-full max-w-[600px] rounded-lg ${skeletonClass}`} />
      </div>

      <div className="flex flex-col items-center gap-2.5 py-2.5">
        <div className={`h-5 w-[150px] rounded ${skeletonClass}`} />
        <div className={`h-5 w-[250px] rounded ${skeletonClass}`} />
      </div>
    </div>
  );
};
