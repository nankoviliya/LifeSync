export interface SparkProps {
  width?: number;
  height?: number;
  className?: string;
}

// TODO(afterwork #5.charts): accept a real time-series and render it. No
// time-series/history endpoint exists today, so this is a fixed decorative
// line for the balance hero. See afterwork.md.
export const Spark = ({ width = 360, height = 60, className }: SparkProps) => {
  return (
    <svg
      width="100%"
      height={height}
      viewBox={`0 0 ${width} ${height}`}
      preserveAspectRatio="none"
      className={className}
      aria-hidden="true"
    >
      <path
        d={`M0,${height * 0.75} C${width * 0.12},${height * 0.68} ${
          width * 0.2
        },${height * 0.35} ${width * 0.32},${height * 0.45} C${width * 0.44},${
          height * 0.55
        } ${width * 0.53},${height * 0.18} ${width * 0.65},${height * 0.28} C${
          width * 0.77
        },${height * 0.38} ${width * 0.88},${height * 0.1} ${width},${
          height * 0.2
        }`}
        fill="none"
        stroke="currentColor"
        strokeWidth={2}
        strokeLinecap="round"
      />
    </svg>
  );
};
